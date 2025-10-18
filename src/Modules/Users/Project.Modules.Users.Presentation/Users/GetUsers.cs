using Project.Common.Presentation.Endpoints;
using Project.Modules.Users.Application.Users.GetUsers;
using FluentResults;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Project.Common.Presentation.Results;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class GetUsers : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users", async (ISender sender) =>
        {
            Result<IReadOnlyCollection<UserResponse>> result = await sender.Send(new GetUsersQuery());

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Users)
        .WithName("GetUsers")
        .WithSummary("Get all users")
        .WithDescription("Retrieves a collection of all users in the system")
        .Produces<IReadOnlyCollection<UserResponse>>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
