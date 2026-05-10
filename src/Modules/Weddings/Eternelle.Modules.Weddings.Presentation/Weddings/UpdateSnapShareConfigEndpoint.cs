using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.UpdateSnapShareConfig;
using Eternelle.Modules.Weddings.Domain.Weddings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class UpdateSnapShareConfigEndpoint : IEndpoint
{
    internal sealed record Request(
        string? InstagramHandle,
        string? CtaText,
        bool Enabled,
        int ModerationMode);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("weddings/{weddingId}/snap-share", async (Guid weddingId, Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateSnapShareConfigCommand(
                weddingId,
                request.InstagramHandle,
                request.CtaText,
                request.Enabled,
                (SnapShareModerationMode)request.ModerationMode);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization("wedding:edit");
    }
}
