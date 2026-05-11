using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.EntourageGroups.AddEntourageMember;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.EntourageGroups;

internal sealed class AddEntourageMemberEndpoint : IEndpoint
{
    internal sealed record Request(
        string Name,
        string Role,
        string? ImageUrl,
        string? Message,
        string? Note,
        int? Seed,
        int DisplayOrder);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("entourage/groups/{groupId}/members", async (
            Guid groupId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new AddEntourageMemberCommand(
                groupId,
                request.Name,
                request.Role,
                request.ImageUrl,
                request.Message,
                request.Note,
                request.Seed,
                request.DisplayOrder);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                memberId => Results.Created($"/entourage/groups/{groupId}/members/{memberId}", new { id = memberId }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Entourage)
        .RequireAuthorization("wedding:edit");
    }
}
