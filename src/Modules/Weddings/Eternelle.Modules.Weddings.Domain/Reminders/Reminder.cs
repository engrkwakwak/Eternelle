using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.Reminders;

/// <summary>
/// A reminder card displayed to guests (wedding.reminders).
/// First-class tier only.
///
/// Reminder is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// display_order is managed explicitly: Create() receives the intended position,
/// and Reorder() handles moves. Bulk reordering is handled by calling Reorder()
/// on each affected reminder within a single transaction.
/// </summary>
public sealed class Reminder : Entity
{
    private Reminder()
    {
    }

    public ReminderId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IReminderRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    /// <summary>
    /// Icon identifier or emoji used in the rendered card.
    /// The domain stores, not interprets, the icon value — rendering is the UI's concern.
    /// </summary>
    public string Icon { get; private set; }

    public string Title { get; private set; }

    public string Body { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static Reminder Create(
        WeddingId weddingId,
        string icon,
        string title,
        string body,
        int displayOrder)
    {
        var reminder = new Reminder
        {
            Id = ReminderId.New(),
            WeddingId = weddingId,
            Icon = icon,
            Title = title,
            Body = body,
            DisplayOrder = displayOrder
        };

        reminder.Raise(new ReminderCreatedDomainEvent(reminder.Id, weddingId));

        return reminder;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public void Update(string icon, string title, string body)
    {
        Icon = icon;
        Title = title;
        Body = body;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
