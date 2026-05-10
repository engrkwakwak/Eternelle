using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.GetWedding;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class GetWeddingEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<WeddingResponse> result = await sender.Send(new GetWeddingQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization("wedding:edit");
    }
}
