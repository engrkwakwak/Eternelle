using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.CeremonyActs;

public interface ICeremonyActRepository
{
    Task<CeremonyAct?> GetAsync(CeremonyActId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<CeremonyAct>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(CeremonyAct ceremonyAct);

    void Update(CeremonyAct ceremonyAct);

    void Delete(CeremonyAct ceremonyAct);
}
