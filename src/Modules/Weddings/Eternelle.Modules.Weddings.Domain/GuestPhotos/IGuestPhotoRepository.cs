using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GuestPhotos;

public interface IGuestPhotoRepository
{
    Task<GuestPhoto?> GetAsync(GuestPhotoId id, CancellationToken ct = default);

    Task<IReadOnlyList<GuestPhoto>> GetManyAsync(IReadOnlyList<GuestPhotoId> ids, CancellationToken ct = default);

    Task<IReadOnlyList<GuestPhoto>> GetByWeddingIdAsync(
        WeddingId weddingId,
        GuestPhotoStatus? status = null,
        CancellationToken ct = default);

    Task<int> CountByWeddingIdAsync(WeddingId weddingId, CancellationToken ct = default);

    Task EnforcePhotoLimitAsync(WeddingId weddingId, int limit, CancellationToken ct = default);

    /// <summary>
    /// Inserts <paramref name="photo"/> and enforces the plan cap in a single transaction,
    /// so no committed photo can exist outside the limit window.
    /// </summary>
    Task InsertAndEnforceAsync(GuestPhoto photo, WeddingId weddingId, int planLimit, CancellationToken ct = default);

    /// <summary>
    /// Inserts all <paramref name="photos"/> and enforces the plan cap in a single transaction.
    /// Prefer this over calling <see cref="InsertAndEnforceAsync"/> in a loop for bulk uploads.
    /// </summary>
    Task InsertManyAndEnforceAsync(IReadOnlyList<GuestPhoto> photos, WeddingId weddingId, int planLimit, CancellationToken ct = default);

    void Insert(GuestPhoto photo);

    void InsertMany(IReadOnlyList<GuestPhoto> photos);

    void Update(GuestPhoto photo);

    void Delete(GuestPhoto photo);
}
