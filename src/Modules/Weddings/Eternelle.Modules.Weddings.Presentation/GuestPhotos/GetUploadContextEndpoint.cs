using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.GuestPhotos.GetUploadContext;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.GuestPhotos;

internal sealed class GetUploadContextEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("photos/upload", async (
            Guid token,
            ISender sender,
            CancellationToken ct) =>
        {
            Result<UploadContextResponse> result =
                await sender.Send(new GetUploadContextQuery(token), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.SnapShare)
        .AllowAnonymous();
    }
}
