using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos;
using Eternelle.Modules.Weddings.Application.GuestPhotos.GetPublicGuestPhotos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class GetPublicGuestPhotoFeedEndpoint : IEndpoint
{
    private sealed record PublicPhotoResponse(
        Guid Id,
        string SrcUrl,
        string? ThumbnailUrl,
        int? WidthPx,
        int? HeightPx,
        string? UploaderName,
        DateTime UploadedAt);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/photos/feed", async (
            Guid weddingId,
            ISender sender,
            HttpContext httpContext,
            CancellationToken ct) =>
        {
            Result<IReadOnlyList<GuestPhotoResponse>> result =
                await sender.Send(new GetPublicGuestPhotosQuery(weddingId), ct);

            return result.Match(
                photos =>
                {
                    httpContext.Response.Headers.CacheControl = "public, max-age=30";

                    IEnumerable<PublicPhotoResponse> publicPhotos = photos.Select(p => new PublicPhotoResponse(
                        p.Id,
                        p.SrcUrl,
                        p.ThumbnailUrl,
                        p.WidthPx,
                        p.HeightPx,
                        p.UploaderName,
                        p.UploadedAt));

                    return Results.Ok(new { photos = publicPhotos });
                },
                ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .AllowAnonymous();
    }
}
