using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.VendorCredits.UpdateVendorCredit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.VendorCredits;

internal sealed class UpdateVendorCreditEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string Role,
        string? WebsiteUrl,
        string? ImageUrl,
        string? InstagramHandle);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/vendor-credits/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateVendorCreditCommand(
                weddingId,
                id,
                request.Name,
                request.Role,
                request.WebsiteUrl,
                request.ImageUrl,
                request.InstagramHandle);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.VendorCredits)
        .RequireAuthorization("wedding:edit");
    }
}
