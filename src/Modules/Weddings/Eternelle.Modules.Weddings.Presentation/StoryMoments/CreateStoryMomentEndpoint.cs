using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.StoryMoments;

internal sealed class CreateStoryMomentEndpoint : IEndpoint
{
    internal sealed record Request(
        string Title,
        DateOnly? StoryDate,
        string Description,
        string? ImageUrl,
        int DisplayOrder);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/story-moments", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateStoryMomentCommand(
                weddingId,
                request.Title,
                request.StoryDate,
                request.Description,
                request.ImageUrl,
                request.DisplayOrder);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/story-moments/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.StoryMoments)
        .RequireAuthorization("wedding:edit");
    }
}
