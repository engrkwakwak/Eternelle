namespace Eternelle.Modules.Weddings.Domain.Weddings;

public interface IWeddingRepository
{
    Task<Wedding?> GetAsync(WeddingId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the Wedding aggregate together with its Partners.
    /// Used when partner mutations are needed.
    /// </summary>
    Task<Wedding?> GetWithPartnersAsync(WeddingId id, CancellationToken cancellationToken = default);

    Task<Wedding?> GetByTenantIdAsync(Guid tenantId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads the Wedding aggregate (including SnapShare) whose SnapShare.UploadToken matches.
    /// Returns null when no match is found — callers must not reveal token validity to the outside.
    /// </summary>
    Task<Wedding?> GetByUploadTokenAsync(Guid uploadToken, CancellationToken cancellationToken = default);

    void Insert(Wedding wedding);

    void Update(Wedding wedding);
}
