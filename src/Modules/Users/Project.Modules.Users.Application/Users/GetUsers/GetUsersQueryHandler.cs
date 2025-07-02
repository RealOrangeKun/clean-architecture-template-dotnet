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
                id AS {nameof(UserDbDto.Id)},
                first_name AS {nameof(UserDbDto.FirstName)},
                last_name AS {nameof(UserDbDto.LastName)},
                email AS {nameof(UserDbDto.Email)}
            FROM users.users
            WHERE role = 'User'
            """;

        IReadOnlyCollection<UserDbDto> users = [.. await dbConnection.QueryAsync<UserDbDto>(sql, request)];

        var usersWithWarnings = users
            .Select(u =>
                new UserResponse(
                        u.Id,
                        u.FirstName,
                        u.LastName,
                        u.Email,
                        0))
            .ToList();

        await cacheService.SetAsync(
            request.ToString(),
            usersWithWarnings,
            TimeSpan.FromMinutes(1),
            cancellationToken);

        return Result.Ok((IReadOnlyCollection<UserResponse>)usersWithWarnings.AsReadOnly())
            .WithCustomSuccess("Users retrieved successfully.");
    }
    private sealed record UserDbDto(int Id, string FirstName, string LastName, string Email);
}

