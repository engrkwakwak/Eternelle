using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.CreateStoryMoment;

public sealed record CreateStoryMomentCommand(
    Guid WeddingId,
    string Title,
    DateOnly? StoryDate,
    string Description,
    string? ImageUrl,
    int DisplayOrder) : ICommand<Guid>;
