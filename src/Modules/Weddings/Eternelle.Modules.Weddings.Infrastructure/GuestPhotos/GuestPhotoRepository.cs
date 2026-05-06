using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;

internal sealed class GuestPhotoRepository(WeddingsDbContext context) : IGuestPhotoRepository
{
    public async Task<GuestPhoto?> GetAsync(GuestPhotoId id, CancellationToken ct = default)
    {
        return await context.GuestPhotos
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<GuestPhoto>> GetByWeddingIdAsync(
        WeddingId weddingId,
        GuestPhotoStatus? status = null,
        CancellationToken ct = default)
    {
        IQueryable<GuestPhoto> query = context.GuestPhotos
            .Where(p => p.WeddingId == weddingId);

        if (status.HasValue)
        {
            query = query.Where(p => p.Status == status.Value);
        }

        return await query.ToListAsync(ct);
    }

    public async Task<int> CountByWeddingIdAsync(WeddingId weddingId, CancellationToken ct = default)
    {
        return await context.GuestPhotos
            .CountAsync(p => p.WeddingId == weddingId, ct);
    }

    public void Insert(GuestPhoto photo)
    {
        context.GuestPhotos.Add(photo);
    }

    public void Update(GuestPhoto photo)
    {
        context.GuestPhotos.Update(photo);
    }

    public void Delete(GuestPhoto photo)
    {
        context.GuestPhotos.Remove(photo);
    }
}
