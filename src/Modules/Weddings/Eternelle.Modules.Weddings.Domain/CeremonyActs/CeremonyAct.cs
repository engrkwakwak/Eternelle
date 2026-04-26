using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

/// <summary>
/// A single step in the ceremony flow (wedding.ceremony_acts).
///
/// CeremonyAct is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// act_time is stored as TimeOnly? — a clock time on the ceremony day, not a
/// timestamp. Nullable because couples sometimes describe acts (e.g. "Entrance")
/// without pinning them to a specific time.
///
/// display_order is managed explicitly: Create() receives the intended position,
/// and Reorder() handles moves. Bulk reordering is handled by calling Reorder()
/// on each affected act within a single transaction.
/// </summary>
public sealed class CeremonyAct : Entity
{
    private CeremonyAct()
    {
    }

    public CeremonyActId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use ICeremonyActRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    public string Name { get; private set; }

    public string? Description { get; private set; }

    /// <summary>
    /// Icon identifier or emoji for the act (e.g. "🕊️" or "rings").
    /// The domain stores, not interprets, the icon value — rendering is the UI's concern.
    /// </summary>
    public string? Icon { get; private set; }

    /// <summary>
    /// Optional clock time for the act. Consistent with wedding.events.event_time.
    /// Null when the couple has not assigned a specific time to this act.
    /// </summary>
    public TimeOnly? ActTime { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static CeremonyAct Create(
        WeddingId weddingId,
        string name,
        string? description,
        string? icon,
        TimeOnly? actTime,
        int displayOrder)
    {
        var act = new CeremonyAct
        {
            Id = CeremonyActId.New(),
            WeddingId = weddingId,
            Name = name,
            Description = description,
            Icon = icon,
            ActTime = actTime,
            DisplayOrder = displayOrder
        };

        act.Raise(new CeremonyActCreatedDomainEvent(act.Id, weddingId));

        return act;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public void Update(
        string name,
        string? description,
        string? icon,
        TimeOnly? actTime)
    {
        Name = name;
        Description = description;
        Icon = icon;
        ActTime = actTime;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
