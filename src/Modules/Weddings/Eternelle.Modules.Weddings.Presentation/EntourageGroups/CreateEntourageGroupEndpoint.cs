using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.CreateEntourageGroup;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class CreateEntourageGroupEndpoint : IEndpoint
{
    internal sealed record Request(
        string Label,
        string? Subtitle,
        int? GroupType,
        int RenderAs,
        int DisplayOrder);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/entourage/groups", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateEntourageGroupCommand(
                weddingId,
                request.Label,
                request.Subtitle,
                request.GroupType,
                request.RenderAs,
                request.DisplayOrder);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/entourage/groups/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
