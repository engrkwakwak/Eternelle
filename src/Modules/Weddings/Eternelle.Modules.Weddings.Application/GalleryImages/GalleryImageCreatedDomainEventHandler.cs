using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.GalleryImages;

namespace Eternelle.Modules.Weddings.Application.GalleryImages;

internal sealed class GalleryImageCreatedDomainEventHandler
    : DomainEventHandler<GalleryImageCreatedDomainEvent>
{
    public override Task Handle(GalleryImageCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
