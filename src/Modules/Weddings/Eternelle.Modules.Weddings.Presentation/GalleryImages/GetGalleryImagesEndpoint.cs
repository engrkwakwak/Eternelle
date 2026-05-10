using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GalleryImages.GetGalleryImages;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GalleryImages;

internal sealed class GetGalleryImagesEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/gallery", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<IReadOnlyList<GalleryImageResponse>> result =
                await sender.Send(new GetGalleryImagesQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Gallery)
        .RequireAuthorization("wedding:edit");
    }
}
