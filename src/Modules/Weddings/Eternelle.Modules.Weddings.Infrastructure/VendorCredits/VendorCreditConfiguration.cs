using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.VendorCredits;

internal sealed class VendorCreditConfiguration : IEntityTypeConfiguration<VendorCredit>
{
    public void Configure(EntityTypeBuilder<VendorCredit> builder)
    {
        builder.ToTable("vendor_credits");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.WeddingId).IsRequired();

        // Composite index supports GetByWeddingIdAsync ordered reads.
        // Covers the WHERE wedding_id = ? ORDER BY display_order, id pattern efficiently.
        builder.HasIndex(v => new { v.WeddingId, v.DisplayOrder, v.Id })
            .HasDatabaseName("ix_vendor_credits_wedding_id_display_order");

        builder.Property(v => v.Name)
            .HasConversion(v => v.Value, v => VendorName.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(VendorName.MaxLength);

        builder.Property(v => v.Role)
            .HasConversion(v => v.Value, v => PersonRole.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(PersonRole.MaxLength);

        builder.Property(v => v.WebsiteUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? WebUrl.FromPersistence(v) : null)
            .HasMaxLength(WebUrl.MaxLength);

        builder.Property(v => v.ImageUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ImageUrl.FromPersistence(v) : null)
            .HasMaxLength(ImageUrl.MaxLength);

        // InstagramHandle — value object with FromPersistence() bypass factory.
        // Stored as plain text; Create() validation was enforced on write.
        builder.Property(v => v.InstagramHandle)
            .HasConversion(
                h => h != null ? h.Value : null,
                v => v != null ? InstagramHandle.FromPersistence(v) : null)
            .HasMaxLength(InstagramHandle.MaxLength);

        builder.Property(v => v.DisplayOrder).IsRequired();
    }
}
