using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.GiftOptions;

internal sealed class GiftOptionRepository(WeddingsDbContext context) : IGiftOptionRepository
{
    public async Task<GiftOption?> GetAsync(GiftOptionId id, CancellationToken cancellationToken = default)
    {
        return await context.GiftOptions
            .SingleOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GiftOption>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.GiftOptions
            .Where(g => g.WeddingId == weddingId)
            .OrderBy(g => g.DisplayOrder)
            .ThenBy(g => g.Id)
            .ToListAsync(cancellationToken);
    }

    public void Insert(GiftOption giftOption)
    {
        context.GiftOptions.Add(giftOption);
    }

    public void Update(GiftOption giftOption)
    {
        context.GiftOptions.Update(giftOption);
    }

    public void Delete(GiftOption giftOption)
    {
        context.GiftOptions.Remove(giftOption);
    }
}
