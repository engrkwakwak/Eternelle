using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.GenerateUploadSlots;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

/// <summary>
/// Step 1 of the guest photo upload flow.
/// The client requests N presigned upload slots. Each slot contains a CDN presigned URL
/// for direct upload and the final CDN URL. The client uploads all files in parallel,
/// then calls <see cref="RegisterGuestPhotosEndpoint"/> to register them.
/// </summary>
internal sealed class GenerateUploadSlotsEndpoint : IEndpoint
{
    internal sealed record Request(int Count);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("photos/upload/presign", async (
            Guid token,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            Result<IReadOnlyList<UploadSlotResponse>> result =
                await sender.Send(new GenerateUploadSlotsCommand(token, request.Count), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .AllowAnonymous();
    }
}
