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

    void Insert(GuestPhoto photo);

    void Update(GuestPhoto photo);

    void Delete(GuestPhoto photo);
}
