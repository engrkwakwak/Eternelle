using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GiftOptions.GetGiftOptions;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GiftOptions;

internal sealed class GetGiftOptionsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/gift-options", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<IReadOnlyList<GiftOptionResponse>> result =
                await sender.Send(new GetGiftOptionsQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.GiftOptions)
        .RequireAuthorization("wedding:edit");
    }
}
