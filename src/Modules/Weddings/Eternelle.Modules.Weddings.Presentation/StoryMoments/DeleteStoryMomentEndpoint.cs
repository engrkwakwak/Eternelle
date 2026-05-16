using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.StoryMoments.DeleteStoryMoment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.StoryMoments;

internal sealed class DeleteStoryMomentEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/story-moments/{id}", async (
            Guid weddingId,
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteStoryMomentCommand(weddingId, id), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.StoryMoments)
        .RequireAuthorization("wedding:edit");
    }
}
