using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.Weddings;

internal sealed class WeddingRepository(WeddingsDbContext context) : IWeddingRepository
{
    public async Task<Wedding?> GetAsync(WeddingId id, CancellationToken cancellationToken = default)
    {
        return await context.Weddings
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wedding?> GetWithPartnersAsync(WeddingId id, CancellationToken cancellationToken = default)
    {
        return await context.Weddings
            .Include(w => w.Partners)
            .SingleOrDefaultAsync(w => w.Id == id, cancellationToken);
    }

    public async Task<Wedding?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default)
    {
        return await context.Weddings
            .SingleOrDefaultAsync(w => w.TenantId == tenantId, cancellationToken);
    }

    public void Insert(Wedding wedding)
    {
        context.Weddings.Add(wedding);
    }

    public void Update(Wedding wedding)
    {
        context.Weddings.Update(wedding);
    }
}
