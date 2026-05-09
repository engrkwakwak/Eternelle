using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.VendorCredits;

internal sealed class VendorCreditRepository(WeddingsDbContext context) : IVendorCreditRepository
{
    public async Task<VendorCredit?> GetAsync(VendorCreditId id, CancellationToken cancellationToken = default)
    {
        return await context.VendorCredits
            .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<VendorCredit>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.VendorCredits
            .Where(v => v.WeddingId == weddingId)
            .OrderBy(v => v.DisplayOrder)
            .ThenBy(v => v.Id)
            .ToListAsync(cancellationToken);
    }

    public void Insert(VendorCredit vendorCredit)
    {
        context.VendorCredits.Add(vendorCredit);
    }

    public void Update(VendorCredit vendorCredit)
    {
        context.VendorCredits.Update(vendorCredit);
    }

    public void Delete(VendorCredit vendorCredit)
    {
        context.VendorCredits.Remove(vendorCredit);
    }
}
