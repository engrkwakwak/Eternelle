using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.Shared;
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

        // Composite index supports GetByWeddingIdAsync ordered reads.
        // Covers the WHERE wedding_id = ? ORDER BY display_order, id pattern efficiently.
        builder.HasIndex(g => new { g.WeddingId, g.DisplayOrder, g.Id })
            .HasDatabaseName("ix_gift_options_wedding_id_display_order");

        builder.Property(g => g.Title)
            .HasConversion(v => v.Value, v => ActivityName.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(ActivityName.MaxLength);

        builder.Property(g => g.Description)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? RichDescription.FromPersistence(v) : null)
            .HasMaxLength(RichDescription.MaxLength);

        // GiftDisplayMode — stored as int. C# enum default ordinal values.
        builder.Property(g => g.DisplayMode)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(g => g.LinkUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? WebUrl.FromPersistence(v) : null)
            .HasMaxLength(WebUrl.MaxLength);

        builder.Property(g => g.ImageUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ImageUrl.FromPersistence(v) : null)
            .HasMaxLength(ImageUrl.MaxLength);

        builder.Property(g => g.QrImageUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ImageUrl.FromPersistence(v) : null)
            .HasMaxLength(ImageUrl.MaxLength);

        builder.Property(g => g.AccountName)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? AccountHolderName.FromPersistence(v) : null)
            .HasMaxLength(AccountHolderName.MaxLength);

        builder.Property(g => g.AccountNumber)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? AccountNumber.FromPersistence(v) : null)
            .HasMaxLength(AccountNumber.MaxLength);

        builder.Property(g => g.AccountType)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? AccountType.FromPersistence(v) : null)
            .HasMaxLength(AccountType.MaxLength);

        builder.Property(g => g.DisplayOrder).IsRequired();
    }


}
