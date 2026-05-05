using Microsoft.AspNetCore.Routing;

namespace Eternelle.Common.Presentation.Endpoints;

public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}
