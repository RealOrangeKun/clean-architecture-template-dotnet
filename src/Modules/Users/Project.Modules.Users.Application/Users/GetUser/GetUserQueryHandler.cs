using System.Data.Common;
using Dapper;
using FluentResults;
using Project.Common.Application.Caching;
using Project.Common.Application.Data;
using Project.Common.Application.Messaging;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Application.Users.GetUser;

internal sealed class GetUserQueryHandler(
    IDbConnectionFactory connectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        UserResponse? user = await cacheService.GetOrCreateAsync(
            request.ToString(),
            async _ => await FetchUserAsync(request, cancellationToken),
            TimeSpan.FromMinutes(1),
            cancellationToken);

        if (user is null)
        {
            return Result.Fail(UserErrors.UserNotFound(request.Id));
        }

        return Result.Ok(user);
    }
    private async Task<UserResponse?> FetchUserAsync(GetUserQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(UserResponse.Id)},
                first_name AS {nameof(UserResponse.FirstName)},
                last_name AS {nameof(UserResponse.LastName)},
                email AS {nameof(UserResponse.Email)},
                role AS {nameof(UserResponse.Role)}
            FROM users.users
            WHERE id = @Id;
            """;

        return await connection.QuerySingleOrDefaultAsync<UserResponse>(sql, request);
    }
}
