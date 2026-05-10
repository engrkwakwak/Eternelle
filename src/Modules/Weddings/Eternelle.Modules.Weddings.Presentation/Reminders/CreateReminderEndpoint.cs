using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Reminders.CreateReminder;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Reminders;

internal sealed class CreateReminderEndpoint : IEndpoint
{
    internal sealed record Request(string Icon, string Title, string Body);

    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("weddings/{weddingId}/reminders", async (
            Guid weddingId,
            Request request,
            ISender sender,
            CancellationToken ct) =>
        {
            var command = new CreateReminderCommand(weddingId, request.Icon, request.Title, request.Body);

            Result<Guid> result = await sender.Send(command, ct);

            return result.Match(
                id => Results.Created($"/weddings/{weddingId}/reminders/{id}", new { id }),
                ApiResults.Problem);
        })
        .WithTags(Tags.Reminders)
        .RequireAuthorization("wedding:edit");
    }
}
