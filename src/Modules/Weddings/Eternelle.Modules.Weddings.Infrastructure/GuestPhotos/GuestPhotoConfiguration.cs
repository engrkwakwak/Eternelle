using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;

internal sealed class GuestPhotoConfiguration : IEntityTypeConfiguration<GuestPhoto>
{
    public void Configure(EntityTypeBuilder<GuestPhoto> builder)
    {
        builder.ToTable("guest_photos");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.WeddingId).IsRequired();
        builder.HasIndex(p => p.WeddingId);

        builder.Property(p => p.SrcUrl).IsRequired();

        builder.Property(p => p.ThumbnailUrl);

        builder.Property(p => p.WidthPx);

        builder.Property(p => p.HeightPx);

        builder.Property(p => p.UploaderName)
            .HasMaxLength(GuestPhoto.MaxUploaderNameLength);

        // GuestPhotoStatus — stored as an int column.
        // Pending = 0, Approved = 1, Rejected = 2 (C# enum default values).
        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(p => p.UploadedAt).IsRequired();

        builder.Property(p => p.ReviewedAt);
    }
}
