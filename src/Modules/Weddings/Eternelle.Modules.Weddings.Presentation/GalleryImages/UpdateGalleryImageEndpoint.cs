using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GalleryImages.UpdateGalleryImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GalleryImages;

internal sealed class UpdateGalleryImageEndpoint : IEndpoint
{
    internal sealed record Request(
        string SrcUrl,
        string AltText,
        int? WidthPx,
        int? HeightPx,
        string? Caption);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/gallery/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateGalleryImageCommand(
                id,
                request.SrcUrl,
                request.AltText,
                request.WidthPx,
                request.HeightPx,
                request.Caption);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Gallery)
        .RequireAuthorization("wedding:edit");
    }
}
