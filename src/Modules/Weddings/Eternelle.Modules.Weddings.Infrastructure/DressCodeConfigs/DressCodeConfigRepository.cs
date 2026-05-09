using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal sealed class DressCodeConfigRepository(WeddingsDbContext context) : IDressCodeConfigRepository
{
    public async Task<DressCodeConfig?> GetAsync(DressCodeConfigId id, CancellationToken cancellationToken = default)
    {
        return await context.DressCodeConfigs
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DressCodeConfig?> GetWithDetailsAsync(DressCodeConfigId id, CancellationToken cancellationToken = default)
    {
        return await GetBaseQuery()
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
    }

    public async Task<DressCodeConfig?> GetWithDetailsByColorIdAsync(DressCodeColorId colorId, CancellationToken cancellationToken = default)
    {
        return await GetBaseQuery()
            .SingleOrDefaultAsync(d => d.Colors.Any(c => c.Id == colorId), cancellationToken);
    }

    public async Task<DressCodeConfig?> GetWithDetailsByImageIdAsync(DressCodeImageId imageId, CancellationToken cancellationToken = default)
    {
        return await GetBaseQuery()
            .SingleOrDefaultAsync(d => d.Images.Any(i => i.Id == imageId), cancellationToken);
    }

    public async Task<DressCodeConfig?> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.DressCodeConfigs
            .SingleOrDefaultAsync(d => d.WeddingId == weddingId, cancellationToken);
    }

    private IQueryable<DressCodeConfig> GetBaseQuery() =>
        context.DressCodeConfigs
            .Include(d => d.Colors.OrderBy(c => c.DisplayOrder).ThenBy(c => c.Id))
            .Include(d => d.Images.OrderBy(i => i.DisplayOrder).ThenBy(i => i.Id))
            .AsSplitQuery();

    public void Insert(DressCodeConfig dressCodeConfig)
    {
        context.DressCodeConfigs.Add(dressCodeConfig);
    }

    public void Update(DressCodeConfig dressCodeConfig)
    {
        context.DressCodeConfigs.Update(dressCodeConfig);
    }
}
