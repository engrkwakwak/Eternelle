using Eternelle.Modules.Weddings.Domain.Weddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.Weddings;

internal static class SnapShareConfigConfiguration
{
    public static void Configure(OwnedNavigationBuilder<Wedding, SnapShareConfig> snapShare)
    {
        snapShare.ToTable("snap_share_configs");

        snapShare.HasKey(s => s.Id);

        snapShare.WithOwner().HasForeignKey(s => s.WeddingId);

        // InstagramHandle — value object with FromPersistence() bypass factory.
        // Stored as plain text; Create() validation was enforced on write.
        snapShare.Property(s => s.InstagramHandle)
            .HasConversion(
                h => h != null ? h.Value : null,
                v => v != null ? InstagramHandle.FromPersistence(v) : null)
            .HasMaxLength(InstagramHandle.MaxLength);

        // Explicitly pin WeddingId to "wedding_id" to prevent EF Core from generating
        // a shadow property collision (id1) when the owned entity also has its own Id column.
        snapShare.Property(s => s.WeddingId)
            .HasColumnName("wedding_id");

        snapShare.Property(s => s.CallToAction)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? CallToAction.FromPersistence(v) : null)
            .HasColumnName("cta_text")
            .HasMaxLength(CallToAction.MaxLength);

        snapShare.Property(s => s.Enabled).IsRequired();

        snapShare.Property(s => s.UploadToken);

        // SnapShareModerationMode — stored as int. Auto = 0, Manual = 1.
        snapShare.Property(s => s.ModerationMode)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(SnapShareModerationMode.Auto);

        snapShare.Property(s => s.UploaderNameRequired)
            .IsRequired()
            .HasDefaultValue(false);

        snapShare.HasIndex(s => s.WeddingId)
            .IsUnique()
            .HasDatabaseName("ix_snap_share_configs_wedding_id");

        // Lookup by upload token on every public guest-photo upload request.
        // Partial + unique: skips NULL rows (SnapShare not yet activated) and
        // enforces that no two weddings share the same active token.
        snapShare.HasIndex(s => s.UploadToken)
            .IsUnique()
            .HasFilter("upload_token IS NOT NULL")
            .HasDatabaseName("ix_snap_share_configs_upload_token");
    }
}
