using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class AddDressCodeImageEndpoint : IEndpoint
{
    internal sealed record Request(string ImageUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dress-code/{id}/images", async (
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddDressCodeImageCommand(id, request.ImageUrl);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                imageId => Results.Created($"/dress-code/{id}/images/{imageId}", new { id = imageId }),
                ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
