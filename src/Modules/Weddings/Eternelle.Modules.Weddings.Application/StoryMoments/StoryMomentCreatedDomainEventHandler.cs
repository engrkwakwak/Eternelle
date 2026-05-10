using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.StoryMoments;

namespace Eternelle.Modules.Weddings.Application.StoryMoments;

internal sealed class StoryMomentCreatedDomainEventHandler
    : DomainEventHandler<StoryMomentCreatedDomainEvent>
{
    public override Task Handle(StoryMomentCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
