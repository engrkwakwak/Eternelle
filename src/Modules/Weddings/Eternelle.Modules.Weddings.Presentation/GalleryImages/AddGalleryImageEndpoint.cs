using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GalleryImages.AddGalleryImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GalleryImages;

internal sealed class AddGalleryImageEndpoint : IEndpoint
{
    internal sealed record Request(
        string SrcUrl,
        string AltText,
        int? WidthPx,
        int? HeightPx,
        string? Caption,
        int DisplayOrder);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/gallery", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddGalleryImageCommand(
                weddingId,
                request.SrcUrl,
                request.AltText,
                request.WidthPx,
                request.HeightPx,
                request.Caption,
                request.DisplayOrder);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/gallery/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Gallery)
        .RequireAuthorization("wedding:edit");
    }
}
