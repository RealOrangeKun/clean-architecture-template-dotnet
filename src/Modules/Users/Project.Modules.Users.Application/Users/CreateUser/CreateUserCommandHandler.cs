using FluentResults;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Application.Abstractions.Data;
using Project.Modules.Users.Application.Abstractions.Security;
using Project.Modules.Users.Application.Abstractions.Users;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Users.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        User? existingUser = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (existingUser is not null)
        {
            return Result.Fail(UserErrors.UserAlreadyExists);
        }

        string hashedPassword = passwordHasher.HashPassword(request.Password);

        var user = User.Create(
            request.FirstName,
            request.LastName,
            request.Email,
            hashedPassword
        );

        await userRepository.AddAsync(user, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Ok(user.Id);
    }
}
