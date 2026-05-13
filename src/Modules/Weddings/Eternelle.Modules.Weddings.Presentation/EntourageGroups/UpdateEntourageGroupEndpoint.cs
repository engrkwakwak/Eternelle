using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageGroup;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class UpdateEntourageGroupEndpoint : IEndpoint
{
    internal sealed record Request(
        string Label,
        string? Subtitle,
        int? GroupType,
        int RenderAs);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/entourage/groups/{groupId}", async (
            Guid weddingId,
            Guid groupId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateEntourageGroupCommand(
                weddingId,
                groupId,
                request.Label,
                request.Subtitle,
                request.GroupType,
                request.RenderAs);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
