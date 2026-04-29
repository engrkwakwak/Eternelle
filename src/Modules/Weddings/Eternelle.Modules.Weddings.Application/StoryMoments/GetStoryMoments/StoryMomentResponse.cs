namespace Eternelle.Modules.Weddings.Application.StoryMoments.GetStoryMoments;

public sealed record StoryMomentResponse(
    Guid Id,
    Guid WeddingId,
    string Title,
    DateOnly? StoryDate,
    string Description,
    string? ImageUrl,
    int DisplayOrder);
