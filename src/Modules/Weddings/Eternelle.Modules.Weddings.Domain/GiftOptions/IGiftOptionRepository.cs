using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GiftOptions;

public interface IGiftOptionRepository
{
    Task<GiftOption?> GetAsync(GiftOptionId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GiftOption>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(GiftOption giftOption);

    void Update(GiftOption giftOption);

    void Delete(GiftOption giftOption);
}
