using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GalleryImages.ReorderGalleryImages;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GalleryImages;

internal sealed class ReorderGalleryImagesEndpoint : IEndpoint
{
    internal sealed record Request(IReadOnlyList<Guid> Ids);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("weddings/{weddingId}/gallery/reorder", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ReorderGalleryImagesCommand(weddingId, request.Ids);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Gallery)
        .RequireAuthorization("wedding:edit");
    }
}
