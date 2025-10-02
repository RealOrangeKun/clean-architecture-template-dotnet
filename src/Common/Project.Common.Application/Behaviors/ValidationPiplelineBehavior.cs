using FluentResults;
using FluentValidation;
using FluentValidation.Results;
using Project.Common.Application.Messaging;
using Project.Common.Domain;
using MediatR;

namespace Project.Common.Application.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ValidationFailure[] failures = await ValidateAsync(request);

        if (failures.Length == 0)
        {
            return await next(cancellationToken);
        }

        Error error = new Error("Validation failed")
            .WithErrorType(ErrorType.Validation)
            .WithMetadata(ErrorMetadata.ValidationFailuresKey, failures);

        if (typeof(TResponse).IsGenericType &&
            typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
        {
            Type valueType = typeof(TResponse).GetGenericArguments()[0];

            System.Reflection.MethodInfo? genericFailMethod = typeof(Result)
                .GetMethods()
                .Where(m => m.Name == nameof(Result.Fail) && m.IsGenericMethodDefinition)
                .FirstOrDefault(m =>
                {
                    System.Reflection.ParameterInfo[] parameters = m.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(IError);
                });

            if (genericFailMethod != null)
            {
                System.Reflection.MethodInfo typedMethod = genericFailMethod.MakeGenericMethod(valueType);
                object failedResult = typedMethod.Invoke(null, [error])!;
                return (TResponse)failedResult;
            }
        }

        if (typeof(TResponse) == typeof(Result))
        {
            var failedResult = Result.Fail(error);
            return (TResponse)(object)failedResult;
        }

        throw new ValidationException(failures);
    }

    private async Task<ValidationFailure[]> ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return [];
        }

        ValidationContext<TRequest> context = new(request);
        ValidationResult[] validationResults = await Task.WhenAll(
            validators.Select(validator => validator.ValidateAsync(context)));

        ValidationFailure[] failures = [.. validationResults
            .Where(result => !result.IsValid)
            .SelectMany(result => result.Errors)];

        return failures;
    }
}
