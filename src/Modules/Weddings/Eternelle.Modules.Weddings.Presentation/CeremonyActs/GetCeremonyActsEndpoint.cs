using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.CeremonyActs.GetCeremonyActs;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.CeremonyActs;

internal sealed class GetCeremonyActsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/ceremony-acts", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<IReadOnlyList<CeremonyActResponse>> result =
                await sender.Send(new GetCeremonyActsQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.CeremonyActs)
        .RequireAuthorization("wedding:edit");
    }
}
