using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.UpdatePartner;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class UpdatePartnerEndpoint : IEndpoint
{
    internal sealed record Request(
        string FirstName,
        string LastName,
        string? Bio,
        string? ImageUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/partners/{partnerId}", async (
            Guid weddingId,
            Guid partnerId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdatePartnerCommand(
                weddingId,
                partnerId,
                request.FirstName,
                request.LastName,
                request.Bio,
                request.ImageUrl);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Weddings)
        .RequireAuthorization("wedding:edit");
    }
}
