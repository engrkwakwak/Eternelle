using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.DeleteStoryMoment;

public sealed record DeleteStoryMomentCommand(Guid WeddingId, Guid StoryMomentId) : ICommand;
