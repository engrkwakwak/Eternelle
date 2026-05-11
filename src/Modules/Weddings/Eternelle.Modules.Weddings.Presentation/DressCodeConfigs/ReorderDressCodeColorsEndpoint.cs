using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.ReorderDressCodeColors;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class ReorderDressCodeColorsEndpoint : IEndpoint
{
    internal sealed record Request(IReadOnlyList<Guid> Ids);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("dress-code/{id}/colors/reorder", async (
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ReorderDressCodeColorsCommand(id, request.Ids);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
