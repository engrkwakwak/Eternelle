using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.UpdateStoryMoment;

public sealed record UpdateStoryMomentCommand(
    Guid StoryMomentId,
    string Title,
    DateOnly? StoryDate,
    string Description,
    string? ImageUrl) : ICommand;
