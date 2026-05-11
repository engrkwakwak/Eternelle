using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.ApproveGuestPhoto;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class ApproveGuestPhotoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("weddings/{weddingId}/photos/{photoId}/approve", async (
            Guid weddingId,
            Guid photoId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new ApproveGuestPhotoCommand(weddingId, photoId), ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
