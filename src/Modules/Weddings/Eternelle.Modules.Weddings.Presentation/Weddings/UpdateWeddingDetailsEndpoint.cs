using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.UpdateWeddingDetails;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class UpdateWeddingDetailsEndpoint : IEndpoint
{
    internal sealed record Request(DateOnly WeddingDate, string? Hashtag);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("weddings/{weddingId}/details", async (Guid weddingId, Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new UpdateWeddingDetailsCommand(weddingId, request.WeddingDate, request.Hashtag);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization("wedding:edit");
    }
}
