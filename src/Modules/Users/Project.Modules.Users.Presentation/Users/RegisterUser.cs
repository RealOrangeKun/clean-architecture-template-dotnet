using FluentResults;
using Project.Common.Presentation.Endpoints;
using Project.Common.Presentation.Results;
using Project.Modules.Users.Application.Users.CreateUser;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Project.Modules.Users.Presentation.Users;

internal sealed class RegisterUser : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/users/register", async (ISender sender, RegisterUserRequest request, LinkGenerator linkGenerator, HttpContext httpContext) =>
        {
            Result<Guid> result = await sender.Send(new CreateUserCommand(
                request.FirstName,
                request.LastName,
                request.Email,
                request.Password
            ));

            return result.Match(
                userId => Results.Created($"/users/{userId}", new { Id = userId }),
                ApiResults.Problem);

        })
        .WithTags(Tags.Users);
    }


    internal sealed record RegisterUserRequest(string Email, string Password, string FirstName, string LastName);
}
