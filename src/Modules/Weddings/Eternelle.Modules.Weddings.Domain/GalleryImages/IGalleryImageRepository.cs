using Eternelle.Modules.Weddings.Domain.Weddings;

namespace Eternelle.Modules.Weddings.Domain.GalleryImages;

public interface IGalleryImageRepository
{
    Task<GalleryImage?> GetAsync(GalleryImageId id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<GalleryImage>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default);

    void Insert(GalleryImage galleryImage);

    void Update(GalleryImage galleryImage);

    void Delete(GalleryImage galleryImage);
}
