using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.DressCodeConfigs;

public interface IDressCodeConfigRepository
{
    Task<DressCodeConfig?> GetAsync(DressCodeConfigId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the config with its Colors and Images collections fully loaded.
    /// Use this overload when modifying the palette or images.
    /// </summary>
    Task<DressCodeConfig?> GetWithDetailsAsync(DressCodeConfigId id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the owning config (with Colors loaded) from a color child ID.
    /// Use when a command targets a color by ID and needs the aggregate root.
    /// </summary>
    Task<DressCodeConfig?> GetWithDetailsByColorIdAsync(DressCodeColorId colorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Resolves the owning config (with Images loaded) from an image child ID.
    /// Use when a command targets an image by ID and needs the aggregate root.
    /// </summary>
    Task<DressCodeConfig?> GetWithDetailsByImageIdAsync(DressCodeImageId imageId, CancellationToken cancellationToken = default);

    Task<DressCodeConfig?> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(DressCodeConfig dressCodeConfig);

    void Update(DressCodeConfig dressCodeConfig);
}
