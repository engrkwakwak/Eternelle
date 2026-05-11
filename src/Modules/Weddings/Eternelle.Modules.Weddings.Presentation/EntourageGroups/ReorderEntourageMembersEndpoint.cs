using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.ReorderEntourageMembers;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class ReorderEntourageMembersEndpoint : IEndpoint
{
    internal sealed record Request(IReadOnlyList<Guid> Ids);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPatch("entourage/groups/{groupId}/members/reorder", async (
            Guid groupId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new ReorderEntourageMembersCommand(groupId, request.Ids);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
