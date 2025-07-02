using Project.Common.Presentation.Endpoints;
using Project.Modules.Users.Application.Users.GetUsers;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Project.Common.Presentation.Results;
using Microsoft.AspNetCore.Http;
using Project.Modules.Users.Domain.Users;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class GetUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            Result<IReadOnlyCollection<UserResponse>> result = await sender.Send(new GetUsersQuery());

            return result.ToApiResult();
        })
        .RequireAuthorization(policy => policy.RequireRole(Role.Admin.ToString()))
        .WithTags(Tags.Users);
    }
}
