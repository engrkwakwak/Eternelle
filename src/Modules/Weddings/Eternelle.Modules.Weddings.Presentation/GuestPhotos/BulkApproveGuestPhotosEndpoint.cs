using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.BulkApproveGuestPhotos;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class BulkApproveGuestPhotosEndpoint : IEndpoint
{
    internal sealed record Request(IReadOnlyList<Guid> GuestPhotoIds);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/photos/bulk-approve", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new BulkApproveGuestPhotosCommand(request.GuestPhotoIds);

            Result<BulkApproveGuestPhotosResponse> result = await sender.Send(command, ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
