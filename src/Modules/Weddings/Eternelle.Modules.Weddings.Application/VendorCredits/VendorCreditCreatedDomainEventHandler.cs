using Eternelle.Common.Application.Messaging;
using Eternelle.Modules.Weddings.Domain.VendorCredits;

namespace Eternelle.Modules.Weddings.Application.VendorCredits;

internal sealed class VendorCreditCreatedDomainEventHandler
    : DomainEventHandler<VendorCreditCreatedDomainEvent>
{
    public override Task Handle(VendorCreditCreatedDomainEvent domainEvent, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }
}
