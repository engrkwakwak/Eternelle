using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.Reminders;

public sealed class ReminderCreatedDomainEvent : DomainEvent
{
    public ReminderCreatedDomainEvent(ReminderId reminderId, WeddingId weddingId)
    {
        ReminderId = reminderId;
        WeddingId = weddingId;
    }

    public ReminderId ReminderId { get; init; }

    public WeddingId WeddingId { get; init; }
}
