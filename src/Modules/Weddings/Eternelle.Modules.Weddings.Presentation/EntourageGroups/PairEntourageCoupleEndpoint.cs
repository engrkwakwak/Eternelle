using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.PairEntourageCouple;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class PairEntourageCoupleEndpoint : IEndpoint
{
    internal sealed record Request(Guid MemberAId, Guid MemberBId, string? Note);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/entourage/groups/{groupId}/couples", async (
            Guid weddingId,
            Guid groupId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new PairEntourageCoupleCommand(weddingId, groupId, request.MemberAId, request.MemberBId, request.Note);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                coupleId => Results.Created($"/weddings/{weddingId}/entourage/groups/{groupId}/couples/{coupleId}", new { id = coupleId }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
