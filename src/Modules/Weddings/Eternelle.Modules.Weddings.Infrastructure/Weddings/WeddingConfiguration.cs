using Eternelle.Modules.Weddings.Domain.Weddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.Weddings;

internal sealed class WeddingConfiguration : IEntityTypeConfiguration<Wedding>
{
    public void Configure(EntityTypeBuilder<Wedding> builder)
    {
        builder.ToTable("profiles");

        builder.HasKey(w => w.Id);

        builder.Property(w => w.TenantId).IsRequired();
        builder.HasIndex(w => w.TenantId).IsUnique();

        builder.Property(w => w.SchemaVersion).IsRequired();

        builder.Property(w => w.WeddingDate).IsRequired();

        // Hashtag is a value object — stored as a single nullable text column.
        // FromPersistence() bypasses Create() validation because the value was
        // already validated before it was written to the database.
        // Note: ?. cannot be used in expression tree lambdas — explicit null check is required.
        builder.Property(w => w.Hashtag)
            .HasConversion(
                h => h != null ? h.Value : null,
                v => v != null ? Hashtag.FromPersistence(v) : null)
            .HasColumnName("hashtag");

        // EF Core snake_cases "CreatedAtUtc" → "created_at_utc".
        // The actual DB column is "created_at" — map explicitly.
        builder.Property(w => w.CreatedAtUtc).HasColumnName("created_at").IsRequired();
        builder.Property(w => w.UpdatedAtUtc).HasColumnName("updated_at").IsRequired();

        // ─── Partners ────────────────────────────────────────────────────────────
        // Owned collection in its own table (wedding.partners).
        // EF Core uses the _partners backing field to load and track the collection.

        builder.OwnsMany(w => w.Partners, partner =>
        {
            partner.ToTable("partners");
            partner.HasKey(p => p.Id);
            partner.WithOwner().HasForeignKey(p => p.WeddingId);

            // PartnerNumber is a C# enum backed by int in the DB
            // (the schema uses CHECK (partner_number IN (1, 2)), not a PG ENUM).
            partner.Property(p => p.PartnerNumber)
                .HasConversion<int>()
                .IsRequired();

            partner.Property(p => p.FirstName).IsRequired();
            partner.Property(p => p.LastName).IsRequired();
            partner.Property(p => p.Bio);
            partner.Property(p => p.ImageUrl);

            partner.HasIndex(p => new { p.WeddingId, p.PartnerNumber }).IsUnique();
        });

        builder.Navigation(w => w.Partners)
            .HasField("_partners")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── SnapShare ───────────────────────────────────────────────────────────
        // Optional 1:1 owned entity in its own table (wedding.snap_share_configs).
        // Null until ConfigureSnapShare() is called for the first time.

        builder.OwnsOne(w => w.SnapShare, snapShare =>
        {
            snapShare.ToTable("snap_share_configs");
            snapShare.HasKey(s => s.Id);
            snapShare.WithOwner().HasForeignKey(s => s.WeddingId);

            snapShare.Property(s => s.WeddingId).HasColumnName("wedding_id");

            snapShare.Property(s => s.InstagramHandle)
                .HasConversion(
                    h => h != null ? h.Value : null,
                    v => v != null ? InstagramHandle.FromPersistence(v) : null)
                .HasColumnName("instagram_handle");

            snapShare.Property(s => s.CtaText).HasColumnName("cta_text");
            snapShare.Property(s => s.Enabled).IsRequired();

            snapShare.HasIndex(s => s.WeddingId).IsUnique()
                .HasDatabaseName("ix_snap_share_configs_wedding_id");
        });

        builder.Navigation(w => w.SnapShare).IsRequired(false);
    }
}
