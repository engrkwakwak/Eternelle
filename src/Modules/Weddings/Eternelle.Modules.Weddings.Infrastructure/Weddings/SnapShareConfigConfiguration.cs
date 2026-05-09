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

        snapShare.Property(s => s.CtaText);

        snapShare.Property(s => s.Enabled).IsRequired();

        snapShare.Property(s => s.UploadToken);

        // SnapShareModerationMode — stored as int. Auto = 0, Manual = 1.
        snapShare.Property(s => s.ModerationMode)
            .HasConversion<int>()
            .IsRequired()
            .HasDefaultValue(SnapShareModerationMode.Auto);

        snapShare.HasIndex(s => s.WeddingId)
            .IsUnique()
            .HasDatabaseName("ix_snap_share_configs_wedding_id");
    }
}
