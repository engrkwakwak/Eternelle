using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GiftOptions.DeleteGiftOption;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GiftOptions;

internal sealed class DeleteGiftOptionEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/gift-options/{id}", async (
            Guid weddingId,
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteGiftOptionCommand(weddingId, id), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.GiftOptions)
        .RequireAuthorization("wedding:edit");
    }
}
