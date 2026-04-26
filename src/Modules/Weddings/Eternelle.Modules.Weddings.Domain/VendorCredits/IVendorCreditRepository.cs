using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.VendorCredits;

public interface IVendorCreditRepository
{
    Task<VendorCredit?> GetAsync(VendorCreditId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<VendorCredit>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(VendorCredit vendorCredit);

    void Update(VendorCredit vendorCredit);

    void Delete(VendorCredit vendorCredit);
}
