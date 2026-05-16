using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.StoryMoments.UpdateStoryMoment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.StoryMoments;

internal sealed class UpdateStoryMomentEndpoint : IEndpoint
{
    internal sealed record Request(
        string Title,
        DateOnly? StoryDate,
        string Description,
        string? ImageUrl);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut("weddings/{weddingId}/story-moments/{id}", async (
            Guid weddingId,
            Guid id,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new UpdateStoryMomentCommand(
                id,
                request.Title,
                request.StoryDate,
                request.Description,
                request.ImageUrl);

            Result result = await sender.Send(command, ct);

            return result.Match(() => Results.Ok(), ApiResults.Problem);
        })
        .WithTags(Tags.StoryMoments)
        .RequireAuthorization("wedding:edit");
    }
}
