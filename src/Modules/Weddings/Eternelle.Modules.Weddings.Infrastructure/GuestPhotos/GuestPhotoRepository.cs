using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;

internal sealed class GuestPhotoRepository(WeddingsDbContext context) : IGuestPhotoRepository
{
    public async Task<GuestPhoto?> GetAsync(GuestPhotoId id, CancellationToken ct = default)
    {
        return await context.GuestPhotos
            .SingleOrDefaultAsync(p => p.Id == id, ct);
    }

    public async Task<IReadOnlyList<GuestPhoto>> GetManyAsync(
        IReadOnlyList<GuestPhotoId> ids,
        CancellationToken ct = default)
    {
        return await context.GuestPhotos
            .Where(p => ids.Contains(p.Id))
            .OrderBy(p => p.Id)
            .ToListAsync(ct);
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

        return await query.OrderBy(p => p.Id).ToListAsync(ct);
    }

    public async Task<int> CountByWeddingIdAsync(WeddingId weddingId, CancellationToken ct = default)
    {
        return await context.GuestPhotos
            .CountAsync(p => p.WeddingId == weddingId, ct);
    }

    public async Task EnforcePhotoLimitAsync(
        WeddingId weddingId,
        int limit,
        CancellationToken ct = default)
    {
        int overLimit = (int)GuestPhotoStatus.OverLimit;

        await context.Database.ExecuteSqlAsync(
            $"""
            UPDATE wedding.guest_photos
               SET status = {overLimit}
             WHERE wedding_id = {weddingId.Value}
               AND status     != {overLimit}
               AND id NOT IN (
                   SELECT id
                     FROM wedding.guest_photos
                    WHERE wedding_id = {weddingId.Value}
                      AND status     != {overLimit}
                    ORDER BY uploaded_at ASC, id ASC
                    LIMIT {limit}
               )
            """,
            ct);
    }

    public async Task InsertAndEnforceAsync(
        GuestPhoto photo,
        WeddingId weddingId,
        int planLimit,
        CancellationToken ct = default)
    {
        await using IDbContextTransaction tx = await context.Database.BeginTransactionAsync(ct);

        try
        {
            context.GuestPhotos.Add(photo);
            await context.SaveChangesAsync(ct);

            await EnforcePhotoLimitAsync(weddingId, planLimit, ct);

            await tx.CommitAsync(ct);
        }
        catch
        {
            await tx.RollbackAsync(ct);
            throw;
        }
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
