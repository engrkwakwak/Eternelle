using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.Reminders;

public sealed class ReminderCreatedDomainEvent(ReminderId reminderId, WeddingId weddingId) : DomainEvent
{
    public ReminderId ReminderId { get; init; } = reminderId;

    public WeddingId WeddingId { get; init; } = weddingId;
}
