using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public sealed class WeddingCreatedDomainEvent(
    WeddingId weddingId,
    Guid tenantId,
    DateOnly weddingDate) : DomainEvent
{
    public WeddingId WeddingId { get; init; } = weddingId;
    public Guid TenantId { get; init; } = tenantId;
    public DateOnly WeddingDate { get; init; } = weddingDate;
}
