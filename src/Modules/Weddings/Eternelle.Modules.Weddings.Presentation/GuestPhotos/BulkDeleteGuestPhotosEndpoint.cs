using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.BulkDeleteGuestPhotos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class BulkDeleteGuestPhotosEndpoint : IEndpoint
{
    internal sealed record Request(IReadOnlyList<Guid> GuestPhotoIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/photos/bulk-delete", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new BulkDeleteGuestPhotosCommand(request.GuestPhotoIds);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
