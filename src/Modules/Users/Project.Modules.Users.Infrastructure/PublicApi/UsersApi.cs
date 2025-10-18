using FluentResults;
using MediatR;
using Project.Modules.Users.Application.Users.GetUser;
using Project.Modules.Users.PublicApi;
using UserResponse = Project.Modules.Users.PublicApi.UserResponse;

namespace Project.Modules.Users.Infrastructure.PublicApi;

public class UsersApi(ISender sender) : IUsersApi
{
    public async Task<UserResponse?> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        Result<Application.Users.GetUser.UserResponse> result = await sender.Send(new GetUserQuery(userId), cancellationToken);

        return result.IsSuccess
            ? new UserResponse(result.Value.Id, result.Value.Email, result.Value.FirstName, result.Value.LastName)
            : null;
    }
}
