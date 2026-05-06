using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.GiftOptions;

internal sealed class GiftOptionConfiguration : IEntityTypeConfiguration<GiftOption>
{
    public void Configure(EntityTypeBuilder<GiftOption> builder)
    {
        builder.ToTable("gift_options");
        builder.HasKey(g => g.Id);

        // WeddingId — cross-aggregate reference.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(g => g.WeddingId).IsRequired();
        builder.HasIndex(g => g.WeddingId)
            .HasDatabaseName("ix_gift_options_wedding_id");

        builder.Property(g => g.Title)
            .IsRequired()
            .HasMaxLength(GiftOption.MaxTitleLength);

        builder.Property(g => g.Description)
            .HasMaxLength(GiftOption.MaxDescriptionLength);

        // GiftDisplayMode — stored as snake_case string to match the PostgreSQL ENUM.
        builder.Property(g => g.DisplayMode)
            .HasConversion(
                v => DisplayModeToString(v),
                v => StringToDisplayMode(v))
            .HasColumnName("display_mode")
            .IsRequired();

        builder.Property(g => g.LinkUrl);
        builder.Property(g => g.ImageUrl);
        builder.Property(g => g.QrImageUrl);

        builder.Property(g => g.AccountName)
            .HasMaxLength(GiftOption.MaxAccountNameLength);

        builder.Property(g => g.AccountNumber)
            .HasMaxLength(GiftOption.MaxAccountNumberLength);

        builder.Property(g => g.AccountType)
            .HasMaxLength(GiftOption.MaxAccountTypeLength);

        builder.Property(g => g.DisplayOrder).IsRequired();
    }

    // ─── Enum ↔ string helpers ───────────────────────────────────────────────────

    private static string DisplayModeToString(GiftDisplayMode value) => value switch
    {
        GiftDisplayMode.Link          => "link",
        GiftDisplayMode.ModalDetails  => "modal_details",
        GiftDisplayMode.InlineDetails => "inline_details",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static GiftDisplayMode StringToDisplayMode(string value) => value switch
    {
        "link"           => GiftDisplayMode.Link,
        "modal_details"  => GiftDisplayMode.ModalDetails,
        "inline_details" => GiftDisplayMode.InlineDetails,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
