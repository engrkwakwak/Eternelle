using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Weddings.RegenerateUploadToken;
using Eternelle.Modules.Weddings.Application.Weddings;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Options;

namespace Eternelle.Modules.Weddings.Presentation.Weddings;

internal sealed class RegenerateUploadTokenEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/snap-share/regenerate-token", async (
            Guid weddingId,
            ISender sender,
            IOptions<SnapShareOptions> options,
            CancellationToken ct) =>
        {
            Result<Guid> result = await sender.Send(new RegenerateUploadTokenCommand(weddingId), ct);

            return result.Match(
                token => Results.Ok(new
                {
                    uploadToken = token,
                    qrUrl = $"{options.Value.UploadBaseUrl}{(options.Value.UploadBaseUrl.Contains('?') ? '&' : '?')}token={token}"
                }),
                ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .RequireAuthorization("wedding:edit");
    }
}
