using Eternelle.Common.Domain;
using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.StoryMoments;

/// <summary>
/// Represents a single moment in the couple's love story (wedding.story_moments).
///
/// StoryMoment is its own aggregate root — it has an independent lifecycle and is
/// always referenced by WeddingId, never navigated through the Wedding aggregate.
///
/// display_order is managed explicitly: the application layer passes the intended
/// order on Create, and Reorder() handles moves. Bulk reordering (drag-and-drop)
/// is handled by calling Reorder() on each affected moment within a single transaction.
/// </summary>
public sealed class StoryMoment : Entity
{
    private StoryMoment()
    {
    }

    public StoryMomentId Id { get; private set; }

    /// <summary>
    /// Cross-aggregate reference to the parent wedding.
    /// Never navigated — use IStoryMomentRepository.GetByWeddingIdAsync() to query.
    /// </summary>
    public WeddingId WeddingId { get; private set; }

    public string Title { get; private set; }

    /// <summary>
    /// The date this moment occurred. Nullable — some couples describe a moment
    /// without pinning it to a specific date (e.g. "How we met").
    /// </summary>
    public DateOnly? StoryDate { get; private set; }

    public string Description { get; private set; }

    public string? ImageUrl { get; private set; }

    public int DisplayOrder { get; private set; }

    // ─── Factory ────────────────────────────────────────────────────────────────

    public static StoryMoment Create(
        WeddingId weddingId,
        string title,
        DateOnly? storyDate,
        string description,
        string? imageUrl,
        int displayOrder)
    {
        var storyMoment = new StoryMoment
        {
            Id = StoryMomentId.New(),
            WeddingId = weddingId,
            Title = title,
            StoryDate = storyDate,
            Description = description,
            ImageUrl = imageUrl,
            DisplayOrder = displayOrder
        };

        storyMoment.Raise(new StoryMomentCreatedDomainEvent(storyMoment.Id, weddingId));

        return storyMoment;
    }

    // ─── Behaviour ──────────────────────────────────────────────────────────────

    public void Update(
        string title,
        DateOnly? storyDate,
        string description,
        string? imageUrl)
    {
        Title = title;
        StoryDate = storyDate;
        Description = description;
        ImageUrl = imageUrl;
    }

    public void Reorder(int displayOrder)
    {
        DisplayOrder = displayOrder;
    }
}
