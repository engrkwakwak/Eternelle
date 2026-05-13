using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.RemoveDressCodeImage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class RemoveDressCodeImageEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("dress-code/{id}/images/{imageId}", async (
            Guid id,
            Guid imageId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new RemoveDressCodeImageCommand(id, imageId), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
