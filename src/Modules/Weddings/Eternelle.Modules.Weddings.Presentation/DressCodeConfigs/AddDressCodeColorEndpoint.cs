using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.DressCodeConfigs.AddDressCodeColor;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.DressCodeConfigs;

internal sealed class AddDressCodeColorEndpoint : IEndpoint
{
    internal sealed record Request(string ColorHex, string ColorName);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("dress-code/{id}/colors", async (
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddDressCodeColorCommand(id, request.ColorHex, request.ColorName);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                colorId => Results.Created($"/dress-code/{id}/colors/{colorId}", new { id = colorId }),
                ApiResults.Problem);
        })
        .WithTags(Tags.DressCode)
        .RequireAuthorization("wedding:edit");
    }
}
