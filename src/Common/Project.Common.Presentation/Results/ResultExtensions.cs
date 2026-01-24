using FluentResults;
using Microsoft.AspNetCore.Http;

namespace Project.Common.Presentation.Results;

public static class ResultExtensions
{
    extension(Result res)
    {
        public TOut Match<TOut>(
            Func<TOut> onSuccess,
            Func<Result, TOut> onFailure)
            => res.IsSuccess ? onSuccess() : onFailure(res);

        public IResult Match(
            IResult onSuccess,
            Func<Result, IResult> onFailure)
            => res.IsSuccess ? onSuccess : onFailure(res);
    }

    extension<TIn>(Result<TIn> res)
    {
        public TOut Match<TOut>(
            Func<TIn, TOut> onSuccess,
            Func<Result<TIn>, TOut> onFailure)
            => res.IsSuccess ? onSuccess(res.Value) : onFailure(res);
    }
}