using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.VendorCredits.DeleteVendorCredit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.VendorCredits;

internal sealed class DeleteVendorCreditEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/vendor-credits/{id}", async (
            Guid weddingId,
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteVendorCreditCommand(id), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.VendorCredits)
        .RequireAuthorization("wedding:edit");
    }
}
