using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos;
using Eternelle.Modules.Weddings.Application.GuestPhotos.GetGuestPhotos;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class GetGuestPhotosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/photos", async (
            Guid weddingId,
            string? status,
            ISender sender,
            CancellationToken ct) =>
        {
            GuestPhotoStatus? parsedStatus = Enum.TryParse<GuestPhotoStatus>(status, ignoreCase: true, out GuestPhotoStatus s) ? s : null;

            Result<IReadOnlyList<GuestPhotoResponse>> result =
                await sender.Send(new GetGuestPhotosQuery(weddingId, parsedStatus), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
