using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.CeremonyActs.CreateCeremonyAct;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.CeremonyActs;

internal sealed class CreateCeremonyActEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string? Description,
        string? Icon,
        TimeOnly? ActTime);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/ceremony-acts", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateCeremonyActCommand(
                weddingId,
                request.Name,
                request.Description,
                request.Icon,
                request.ActTime);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/ceremony-acts/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.CeremonyActs)
        .RequireAuthorization("wedding:edit");
    }
}
