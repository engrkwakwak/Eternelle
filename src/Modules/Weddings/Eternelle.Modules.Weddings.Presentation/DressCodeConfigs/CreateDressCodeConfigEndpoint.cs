using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.CreateDressCodeConfig;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class CreateDressCodeConfigEndpoint : IEndpoint
{
    internal sealed record Request(string Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/dress-code", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateDressCodeConfigCommand(weddingId, request.Description);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/dress-code/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
