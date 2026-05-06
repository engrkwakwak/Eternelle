using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.GalleryImages;

internal sealed class GalleryImageConfiguration : IEntityTypeConfiguration<GalleryImage>
{
    public void Configure(EntityTypeBuilder<GalleryImage> builder)
    {
        builder.ToTable("gallery_images");
        builder.HasKey(g => g.Id);

        // WeddingId — cross-aggregate reference.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(g => g.WeddingId).IsRequired();
        builder.HasIndex(g => g.WeddingId)
            .HasDatabaseName("ix_gallery_images_wedding_id");

        builder.Property(g => g.SrcUrl).IsRequired();

        builder.Property(g => g.AltText)
            .IsRequired()
            .HasMaxLength(GalleryImage.MaxAltTextLength);

        builder.Property(g => g.WidthPx);
        builder.Property(g => g.HeightPx);

        builder.Property(g => g.Caption)
            .HasMaxLength(GalleryImage.MaxCaptionLength);

        builder.Property(g => g.DisplayOrder).IsRequired();

        builder.Property(g => g.CreatedAtUtc).IsRequired();
    }
}
