using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.UploadGuestPhoto;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class UploadGuestPhotoEndpoint : IEndpoint
{
    internal sealed record Request(
        Guid UploadToken,
        string SrcUrl,
        string? ThumbnailUrl,
        string? UploaderName,
        int? WidthPx,
        int? HeightPx);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("guest-photos/upload", async (Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new UploadGuestPhotoCommand(
                request.UploadToken,
                request.SrcUrl,
                request.ThumbnailUrl,
                request.UploaderName,
                request.WidthPx,
                request.HeightPx);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/guest-photos/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .AllowAnonymous();
    }
}
