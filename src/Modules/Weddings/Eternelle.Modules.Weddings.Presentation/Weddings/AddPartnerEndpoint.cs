using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.AddPartner;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class AddPartnerEndpoint : IEndpoint
{
    internal sealed record Request(
        int PartnerNumber,
        string FirstName,
        string LastName,
        string? Bio,
        string? ImageUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/partners", async (Guid weddingId, Request request, ISender sender, CancellationToken ct) =>
        {
            var command = new AddPartnerCommand(
                weddingId,
                request.PartnerNumber,
                request.FirstName,
                request.LastName,
                request.Bio,
                request.ImageUrl);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                partnerId => Results.Created($"/weddings/{weddingId}/partners/{partnerId}", new { id = partnerId }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization("wedding:edit");
    }
}
