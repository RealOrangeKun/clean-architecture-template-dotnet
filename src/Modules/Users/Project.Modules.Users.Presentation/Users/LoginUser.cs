using FluentResults;
using Project.Common.Presentation.Endpoints;
using Project.Common.Presentation.Results;
using Project.Modules.Users.Application.Users.LoginUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Mvc;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class LoginUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/login", async (ISender sender, LoginUserRequest request) =>
        {
            Result<LoginUserResponse> result = await sender.Send(new LoginUserCommand(request.Email, request.Password));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithName(nameof(LoginUser))
        .WithSummary("Authenticate user")
        .WithDescription("Authenticates a user with email and password, returns access token.")
        .Produces<LoginUserResponse>(StatusCodes.Status200OK)
        .ProducesProblem(StatusCodes.Status400BadRequest)
        .ProducesProblem(StatusCodes.Status401Unauthorized)
        .ProducesProblem(StatusCodes.Status500InternalServerError)
        .WithTags(Tags.Users);
    }

    internal sealed record LoginUserRequest(string Email, string Password);
}
