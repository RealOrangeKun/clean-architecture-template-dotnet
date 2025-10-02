using FluentResults;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Application.Abstractions.Security;
using Project.Modules.Users.Application.Abstractions.Users;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Users.LoginUser;

internal sealed class LoginUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IJwtTokenGenerator jwtTokenGenerator) : ICommandHandler<LoginUserCommand, LoginUserResponse>
{
    public async Task<Result<LoginUserResponse>> Handle(LoginUserCommand request, CancellationToken cancellationToken)
    {
        User? user = await userRepository.GetByEmailAsync(request.Email, cancellationToken);

        if (user is null || !passwordHasher.VerifyHashedPassword(user.HashedPassword, request.Password))
        {
            return Result.Fail(UserErrors.CredentialsNotCorrect);
        }

        string accessToken = jwtTokenGenerator.GenerateToken(user.Id, user.Role.ToString());

        var loginResponse = new LoginUserResponse(accessToken);

        return Result.Ok(loginResponse);
    }
}
