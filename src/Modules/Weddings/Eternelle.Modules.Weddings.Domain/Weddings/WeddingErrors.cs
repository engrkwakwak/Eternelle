using Eternelle.Common.Domain;

namespace Eternelle.Modules.Weddings.Domain.Weddings;

public static class WeddingErrors
{
    public static Error NotFound(WeddingId id) =>
        Error.NotFound(
            "Weddings.NotFound",
            $"The wedding with the identifier {id.Value} was not found");

    public static Error TenantAlreadyHasWedding(Guid tenantId) =>
        Error.Conflict(
            "Weddings.TenantAlreadyHasWedding",
            $"A wedding for tenant {tenantId} already exists");

    public static Error PartnerAlreadyExists(PartnerNumber partnerNumber) =>
        Error.Conflict(
            "Weddings.PartnerAlreadyExists",
            $"Partner {partnerNumber} already exists for this wedding");

    public static Error PartnerNotFound(PartnerId partnerId) =>
        Error.NotFound(
            "Weddings.PartnerNotFound",
            $"The partner with the identifier {partnerId.Value} was not found");
}
