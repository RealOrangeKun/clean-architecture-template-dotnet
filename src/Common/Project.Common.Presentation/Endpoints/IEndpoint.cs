using Microsoft.AspNetCore.Routing;

namespace Project.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
