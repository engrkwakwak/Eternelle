using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.UpdateEntourageMember;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class UpdateEntourageMemberEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string Role,
        string? ImageUrl,
        string? Message,
        string? Note,
        int? Seed);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/entourage/groups/{groupId}/members/{memberId}", async (
            Guid weddingId,
            Guid groupId,
            Guid memberId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateEntourageMemberCommand(
                weddingId,
                groupId,
                memberId,
                request.Name,
                request.Role,
                request.ImageUrl,
                request.Message,
                request.Note,
                request.Seed);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
