using System.Data.Common;
using Dapper;
using FluentResults;
using Project.Common.Application.Caching;
using Project.Common.Application.Data;
using Project.Common.Application.Messaging;
using Project.Common.Domain;
using Project.Modules.Users.Domain.Users;

using Microsoft.AspNetCore.Http;

namespace Project.Modules.Users.Application.Users.GetUser;

internal sealed class GetUserQueryHandler(
    IDbConnectionFactory connectionFactory,
    ICacheService cacheService)
    : IQueryHandler<GetUserQuery, UserResponse>
{
    public async Task<Result<UserResponse>> Handle(GetUserQuery request, CancellationToken cancellationToken)
    {
        UserResponse? cacheResult = await cacheService.GetAsync<UserResponse>(
            request.ToString(),
            cancellationToken);

        if (cacheResult is not null)
        {
            return Result.Ok(cacheResult)
                .WithCustomSuccess("User retrieved from cache.", StatusCodes.Status200OK);
        }

        await using DbConnection connection = await connectionFactory.OpenConnectionAsync(cancellationToken);

        const string sql =
            $"""
            SELECT
                id AS {nameof(UserDbDto.Id)},
                first_name AS {nameof(UserDbDto.FirstName)},
                last_name AS {nameof(UserDbDto.LastName)},
                email AS {nameof(UserDbDto.Email)},
                role AS {nameof(UserDbDto.Role)}
            FROM users.users
            WHERE id = @Id;
            """;

        UserDbDto? user = await connection.QuerySingleOrDefaultAsync<UserDbDto>(sql, request);

        if (user is null)
        {
            return Result.Fail(UserErrors.UserNotFound(request.Id));
        }

        var userResponse = new UserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Email,
            user.Role,
            [],
            0);

        await cacheService.SetAsync(
            request.ToString(),
            userResponse,
            TimeSpan.FromMinutes(1),
            cancellationToken);

        return Result.Ok(userResponse)
        .WithCustomSuccess("User retrieved successfully.", StatusCodes.Status200OK);
    }
    internal sealed record UserDbDto(int Id, string FirstName, string LastName, string Email, string Role);

}
