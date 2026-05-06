using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;

internal sealed class EntourageGroupRepository(WeddingsDbContext context) : IEntourageGroupRepository
{
    public async Task<EntourageGroup?> GetAsync(EntourageGroupId id, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .SingleOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<EntourageGroup?> GetWithMembersAsync(EntourageGroupId id, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .Include(g => g.Members)
            .Include(g => g.Couples)
            .SingleOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<EntourageGroup?> GetWithMembersByMemberIdAsync(EntourageMemberId memberId, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .Include(g => g.Members)
            .Include(g => g.Couples)
            .SingleOrDefaultAsync(g => g.Members.Any(m => m.Id == memberId), cancellationToken);
    }

    public async Task<EntourageGroup?> GetWithMembersByCoupleIdAsync(EntourageCoupleId coupleId, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .Include(g => g.Members)
            .Include(g => g.Couples)
            .SingleOrDefaultAsync(g => g.Couples.Any(c => c.Id == coupleId), cancellationToken);
    }

    public async Task<IReadOnlyList<EntourageGroup>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .Where(g => g.WeddingId == weddingId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<EntourageGroup>> GetByWeddingIdWithMembersAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.EntourageGroups
            .Include(g => g.Members)
            .Include(g => g.Couples)
            .Where(g => g.WeddingId == weddingId)
            .ToListAsync(cancellationToken);
    }

    public void Insert(EntourageGroup entourageGroup)
    {
        context.EntourageGroups.Add(entourageGroup);
    }

    public void Update(EntourageGroup entourageGroup)
    {
        context.EntourageGroups.Update(entourageGroup);
    }

    public void Delete(EntourageGroup entourageGroup)
    {
        context.EntourageGroups.Remove(entourageGroup);
    }
}
