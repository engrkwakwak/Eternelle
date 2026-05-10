using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Reminders.DeleteReminder;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Reminders;

internal sealed class DeleteReminderEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete("weddings/{weddingId}/reminders/{id}", async (
            Guid weddingId,
            Guid id,
            ISender sender,
            CancellationToken ct) =>
        {
            Result result = await sender.Send(new DeleteReminderCommand(id), ct);

            return result.Match(() => Results.NoContent(), ApiResults.Problem);
        })
        .WithTags(Tags.Reminders)
        .RequireAuthorization("wedding:edit");
    }
}
