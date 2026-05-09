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
            .IsRequired()
            .HasMaxLength(VendorCredit.MaxNameLength);

        builder.Property(v => v.Role)
            .IsRequired()
            .HasMaxLength(VendorCredit.MaxRoleLength);

        builder.Property(v => v.WebsiteUrl);

        builder.Property(v => v.ImageUrl);

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
