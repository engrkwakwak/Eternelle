using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.RegenerateUploadToken;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class RegenerateUploadTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/snap-share/regenerate-token", async (
            Guid weddingId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result<Guid> result = await sender.Send(new RegenerateUploadTokenCommand(weddingId), ct);

            return result.Match(
                token => Results.Ok(new
                {
                    uploadToken = token,
                    qrUrl = $"https://app.eternelle.ph/photos/upload?token={token}"
                }),
                ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
