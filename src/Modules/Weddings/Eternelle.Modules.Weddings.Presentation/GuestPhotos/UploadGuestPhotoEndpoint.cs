using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.RegisterGuestPhotos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

/// <summary>
/// Step 2 of the guest photo upload flow.
/// The client calls this after uploading files to the CDN using the presigned URLs
/// issued by <see cref="GenerateUploadSlotsEndpoint"/>. Registers all photos in a
/// single batch — one transaction, one enforcement pass.
/// </summary>
internal sealed class RegisterGuestPhotosEndpoint : IEndpoint
{
    internal sealed record PhotoRequest(
        Guid SlotId,
        string? UploaderName,
        int? WidthPx,
        int? HeightPx);

    internal sealed record Request(IReadOnlyList<PhotoRequest> Photos);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("photos/upload", async (
            Guid token,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new RegisterGuestPhotosCommand(
                token,
                request.Photos
                    .Select(p => new PhotoRegistration(p.SlotId, p.UploaderName, p.WidthPx, p.HeightPx))
                    .ToList());

            Result<IReadOnlyList<Guid>> result = await sender.Send(command, ct);

            return result.Match(
                photoIds => Results.Created("/photos/upload", new { photoIds }),
                ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .AllowAnonymous()
        .RequireRateLimiting(RateLimitingPolicies.GuestPhotoUpload);
    }
}
