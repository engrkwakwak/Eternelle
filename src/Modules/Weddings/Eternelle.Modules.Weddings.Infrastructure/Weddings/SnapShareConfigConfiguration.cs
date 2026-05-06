using Eternelle.Modules.Weddings.Domain.Weddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.Weddings;

internal sealed class SnapShareConfigConfiguration
{
    public static void Configure(OwnedNavigationBuilder<Wedding, SnapShareConfig> snapShare)
    {
        snapShare.ToTable("snap_share_configs");
        snapShare.HasKey(s => s.Id);

        // wedding_id FK — cascade delete is the EF Core default for owned entities.
        snapShare.WithOwner().HasForeignKey(s => s.WeddingId);

        snapShare.Property(s => s.WeddingId).HasColumnName("wedding_id");

        // InstagramHandle value object — stored as a plain text column.
        // FromPersistence() bypasses Create() validation because the value was
        // already validated before it was written to the database.
        snapShare.Property(s => s.InstagramHandle)
            .HasConversion(
                h => h != null ? h.Value : null,
                v => v != null ? InstagramHandle.FromPersistence(v) : null)
            .HasColumnName("instagram_handle")
            .HasMaxLength(InstagramHandle.MaxLength);

        snapShare.Property(s => s.CtaText).HasColumnName("cta_text");
        snapShare.Property(s => s.Enabled).IsRequired();
        snapShare.Property(s => s.UploadToken);

        // ModerationMode — stored as snake_case text string.
        snapShare.Property(s => s.ModerationMode)
            .HasConversion(
                v => ModerationModeToString(v),
                v => StringToModerationMode(v))
            .HasColumnName("moderation_mode")
            .IsRequired();

        // UNIQUE on wedding_id enforces the 1:1 relationship with wedding.profiles.
        snapShare.HasIndex(s => s.WeddingId).IsUnique()
            .HasDatabaseName("ix_snap_share_configs_wedding_id");
    }

    // ─── Enum ↔ string helpers ───────────────────────────────────────────────────

    private static string ModerationModeToString(SnapShareModerationMode value) => value switch
    {
        SnapShareModerationMode.Auto   => "auto",
        SnapShareModerationMode.Manual => "manual",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static SnapShareModerationMode StringToModerationMode(string value) => value switch
    {
        "auto"   => SnapShareModerationMode.Auto,
        "manual" => SnapShareModerationMode.Manual,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
