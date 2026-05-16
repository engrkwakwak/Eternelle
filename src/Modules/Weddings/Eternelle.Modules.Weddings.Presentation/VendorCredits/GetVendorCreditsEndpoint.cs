using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.VendorCredits.GetVendorCredits;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.VendorCredits;

internal sealed class GetVendorCreditsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/vendor-credits", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<IReadOnlyList<VendorCreditResponse>> result =
                await sender.Send(new GetVendorCreditsQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.VendorCredits)
        .RequireAuthorization("wedding:edit");
    }
}
