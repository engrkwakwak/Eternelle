using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.DeleteGuestPhoto;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class DeleteGuestPhotoEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/photos/{photoId}", async (
            Guid weddingId,
            Guid photoId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteGuestPhotoCommand(photoId), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
