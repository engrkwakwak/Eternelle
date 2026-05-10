using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.UpdateDressCodeConfig;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class UpdateDressCodeConfigEndpoint : IEndpoint
{
    internal sealed record Request(string Description);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/dress-code/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateDressCodeConfigCommand(id, request.Description);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
