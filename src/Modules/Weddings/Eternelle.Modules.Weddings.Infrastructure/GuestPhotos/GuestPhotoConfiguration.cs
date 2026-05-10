using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;

internal sealed class GuestPhotoConfiguration : IEntityTypeConfiguration<GuestPhoto>
{
    public void Configure(EntityTypeBuilder<GuestPhoto> builder)
    {
        builder.ToTable("guest_photos", "wedding");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.WeddingId).IsRequired();

        builder.Property(p => p.SrcUrl).IsRequired();

        builder.Property(p => p.ThumbnailUrl);

        builder.Property(p => p.WidthPx);

        builder.Property(p => p.HeightPx);

        builder.Property(p => p.UploaderName)
            .HasMaxLength(GuestPhoto.MaxUploaderNameLength);

        // GuestPhotoStatus — stored as int. Pending = 0, Approved = 1, Rejected = 2, OverLimit = 3.
        builder.Property(p => p.Status)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(GuestPhotoStatus.Pending);

        builder.Property(p => p.UploadedAt).IsRequired();

        builder.Property(p => p.ReviewedAt);

        // (wedding_id, status, uploaded_at DESC) — status-filtered reads (feed, moderation queue).
        builder.HasIndex(p => new { p.WeddingId, p.Status, p.UploadedAt })
            .HasDatabaseName("idx_guest_photos_wedding_status_uploaded")
            .IsDescending(false, false, true);

        // (wedding_id, uploaded_at DESC) — unfiltered reads; status column blocks ordered tail above.
        builder.HasIndex(p => new { p.WeddingId, p.UploadedAt })
            .HasDatabaseName("idx_guest_photos_wedding_uploaded")
            .IsDescending(false, true);
    }
}

