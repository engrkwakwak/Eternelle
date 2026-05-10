using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.CeremonyActs.UpdateCeremonyAct;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.CeremonyActs;

internal sealed class UpdateCeremonyActEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string? Description,
        string? Icon,
        TimeOnly? ActTime);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/ceremony-acts/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateCeremonyActCommand(
                id,
                request.Name,
                request.Description,
                request.Icon,
                request.ActTime);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.CeremonyActs)
        .RequireAuthorization("wedding:edit");
    }
}
