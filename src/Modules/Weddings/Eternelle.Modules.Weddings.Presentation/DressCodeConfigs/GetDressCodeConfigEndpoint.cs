using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.GetDressCodeConfig;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class GetDressCodeConfigEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/dress-code", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<DressCodeConfigResponse> result = await sender.Send(new GetDressCodeConfigQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
