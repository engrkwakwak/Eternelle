using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.VendorCredits;

internal sealed class VendorCreditConfiguration : IEntityTypeConfiguration<VendorCredit>
{
    public void Configure(EntityTypeBuilder<VendorCredit> builder)
    {
        builder.ToTable("vendor_credits");
        builder.HasKey(v => v.Id);

        // WeddingId — cross-aggregate reference.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(v => v.WeddingId).IsRequired();
        builder.HasIndex(v => v.WeddingId)
            .HasDatabaseName("ix_vendor_credits_wedding_id");

        builder.Property(v => v.Name)
            .IsRequired()
            .HasMaxLength(VendorCredit.MaxNameLength);

        builder.Property(v => v.Role)
            .IsRequired()
            .HasMaxLength(VendorCredit.MaxRoleLength);

        builder.Property(v => v.WebsiteUrl);
        builder.Property(v => v.ImageUrl);

        // InstagramHandle value object — stored as a plain text column.
        // FromPersistence() bypasses Create() validation because the value was
        // already validated before it was written to the database.
        builder.Property(v => v.InstagramHandle)
            .HasConversion(
                h => h != null ? h.Value : null,
                v => v != null ? InstagramHandle.FromPersistence(v) : null)
            .HasColumnName("instagram_handle")
            .HasMaxLength(InstagramHandle.MaxLength);

        builder.Property(v => v.DisplayOrder).IsRequired();
    }
}
