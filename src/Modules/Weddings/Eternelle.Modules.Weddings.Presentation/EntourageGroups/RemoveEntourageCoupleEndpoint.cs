using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageCouple;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class RemoveEntourageCoupleEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/entourage/groups/{groupId}/couples/{coupleId}", async (
            Guid weddingId,
            Guid groupId,
            Guid coupleId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new RemoveEntourageCoupleCommand(groupId, coupleId), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
