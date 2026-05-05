using Eternelle.Common.Application.Messaging;

namespace Eternelle.Modules.Weddings.Application.StoryMoments.GetStoryMoments;

public sealed record GetStoryMomentsQuery(Guid WeddingId) : IQuery<IReadOnlyList<StoryMomentResponse>>;
