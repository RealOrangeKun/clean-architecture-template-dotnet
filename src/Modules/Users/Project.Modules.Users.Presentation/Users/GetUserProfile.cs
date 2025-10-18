using System.Security.Claims;
using FluentResults;
using Project.Common.Infrastructure.Authentication;
using Project.Common.Presentation.Endpoints;
using Project.Common.Presentation.Results;
using Project.Modules.Users.Application.Users.GetUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class GetUserProfile : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/users/profile", async (ISender sender, ClaimsPrincipal claimsPrincipal) =>
        {
            Result<UserResponse> result = await sender.Send(new GetUserQuery(claimsPrincipal.GetUserId()));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags(Tags.Users)
        .WithName("GetUserProfile")
        .WithSummary("Get current user profile")
        .WithDescription("Retrieves the profile information for the currently authenticated user")
        .Produces<UserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status404NotFound)
        .ProducesProblem(StatusCodes.Status500InternalServerError);
    }
}
