using FluentResults;
using Project.Common.Presentation.Endpoints;
using Project.Common.Presentation.Results;
using Project.Modules.Users.Application.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class GetUserById : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/{id}", async (ISender sender, Guid id) =>
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(id));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithName(nameof(GetUserById))
        .WithTags(Tags.Users)
        .WithSummary("Get user by ID")
        .WithDescription("Retrieves a specific user by their unique identifier")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}