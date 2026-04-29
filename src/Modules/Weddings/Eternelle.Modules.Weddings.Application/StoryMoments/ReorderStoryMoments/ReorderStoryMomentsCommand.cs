using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.ReorderStoryMoments;

public sealed record ReorderStoryMomentsCommand(
    Guid WeddingId,
    IReadOnlyList<Guid> StoryMomentIds) : ICommand;
