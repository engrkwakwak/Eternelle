using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.CeremonyActs;

internal sealed class CeremonyActRepository(WeddingsDbContext context) : ICeremonyActRepository
{
    public async Task<CeremonyAct?> GetAsync(CeremonyActId id, CancellationToken cancellationToken = default)
    {
        return await context.CeremonyActs
            .SingleOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<CeremonyAct>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.CeremonyActs
            .Where(c => c.WeddingId == weddingId)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public void Insert(CeremonyAct ceremonyAct)
    {
        context.CeremonyActs.Add(ceremonyAct);
    }

    public void Update(CeremonyAct ceremonyAct)
    {
        context.CeremonyActs.Update(ceremonyAct);
    }

    public void Delete(CeremonyAct ceremonyAct)
    {
        context.CeremonyActs.Remove(ceremonyAct);
    }
}
