using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.GalleryImages;

internal sealed class GalleryImageRepository(WeddingsDbContext context) : IGalleryImageRepository
{
    public async Task<GalleryImage?> GetAsync(GalleryImageId id, CancellationToken cancellationToken = default)
    {
        return await context.GalleryImages
            .SingleOrDefaultAsync(g => g.Id == id, cancellationToken);
    }

    public async Task<IReadOnlyList<GalleryImage>> GetByWeddingIdAsync(WeddingId weddingId, CancellationToken cancellationToken = default)
    {
        return await context.GalleryImages
            .Where(g => g.WeddingId == weddingId)
            .ToListAsync(cancellationToken);
    }

    public void Insert(GalleryImage galleryImage)
    {
        context.GalleryImages.Add(galleryImage);
    }

    public void Update(GalleryImage galleryImage)
    {
        context.GalleryImages.Update(galleryImage);
    }

    public void Delete(GalleryImage galleryImage)
    {
        context.GalleryImages.Remove(galleryImage);
    }
}
