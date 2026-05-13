using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.RemoveEntourageMember;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class RemoveEntourageMemberEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/entourage/groups/{groupId}/members/{memberId}", async (
            Guid weddingId,
            Guid groupId,
            Guid memberId,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new RemoveEntourageMemberCommand(weddingId, groupId, memberId), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
