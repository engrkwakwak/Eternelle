using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.VendorCredits.CreateVendorCredit;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.VendorCredits;

internal sealed class CreateVendorCreditEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string Role,
        string? WebsiteUrl,
        string? ImageUrl,
        string? InstagramHandle);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/vendor-credits", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateVendorCreditCommand(
                weddingId,
                request.Name,
                request.Role,
                request.WebsiteUrl,
                request.ImageUrl,
                request.InstagramHandle);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/vendor-credits/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.VendorCredits)
        .RequireAuthorization("wedding:edit");
    }
}
