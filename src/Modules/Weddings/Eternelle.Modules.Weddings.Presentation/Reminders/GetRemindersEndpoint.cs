using Eternelle.Common.Domain;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Common.Presentation.Results;
using Eternelle.Modules.Weddings.Application.Reminders.GetReminders;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Eternelle.Modules.Weddings.Presentation.Reminders;

internal sealed class GetRemindersEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("weddings/{weddingId}/reminders", async (Guid weddingId, ISender sender, CancellationToken ct) =>
        {
            Result<IReadOnlyList<ReminderResponse>> result =
                await sender.Send(new GetRemindersQuery(weddingId), ct);

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .WithTags(Tags.Reminders)
        .RequireAuthorization("wedding:edit");
    }
}
