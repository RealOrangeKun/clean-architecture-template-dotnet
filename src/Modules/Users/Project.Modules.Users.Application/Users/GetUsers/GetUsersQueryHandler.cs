using System.Data.Common;
using Dapper;
using FluentResults;
using Project.Common.Application.Caching;
using Project.Common.Application.Data;
using Project.Common.Application.Messaging;
using Project.Common.Domain;

namespace Project.Modules.Users.Application.Users.GetUsers;

internal sealed class GetUsersQueryHandler(
    IDbConnectionFactory dbConnectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetUsersQuery, IReadOnlyCollection<UserResponse>>
{

    public async Task<Result<IReadOnlyCollection<UserResponse>>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        IReadOnlyCollection<UserResponse>? cacheResult = await cacheService.GetAsync<IReadOnlyCollection<UserResponse>>(
            request.ToString(),
            cancellationToken);
        if (cacheResult is not null)
        {
            return Result.Ok(cacheResult)
                .WithCustomSuccess("Users retrieved from cache.");
        }
        await using DbConnection dbConnection = await dbConnectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(UserResponse.Id)},
                first_name AS {nameof(UserResponse.FirstName)},
                last_name AS {nameof(UserResponse.LastName)},
                email AS {nameof(UserResponse.Email)}
            FROM users.users
            WHERE role = 'User'
            """;

        IReadOnlyCollection<UserResponse> users = [.. await dbConnection.QueryAsync<UserResponse>(sql, request)];

        var usersWithWarnings = users
            .Select(u =>
                new UserResponse(
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email))
            .ToList();

        await cacheService.SetAsync(
            request.ToString(),
            usersWithWarnings,
            TimeSpan.FromMinutes(1),
            cancellationToken);

        return Result.Ok((IReadOnlyCollection<UserResponse>)usersWithWarnings.AsReadOnly())
            .WithCustomSuccess("Users retrieved successfully.");
    }
}

