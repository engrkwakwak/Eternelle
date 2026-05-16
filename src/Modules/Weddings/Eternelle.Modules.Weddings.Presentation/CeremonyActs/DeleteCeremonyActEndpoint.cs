using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.CeremonyActs.DeleteCeremonyAct;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.CeremonyActs;

internal sealed class DeleteCeremonyActEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/ceremony-acts/{id}", async (
            Guid weddingId,
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteCeremonyActCommand(weddingId, id), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.CeremonyActs)
        .RequireAuthorization("wedding:edit");
    }
}
