using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.CeremonyActs;

internal sealed class CeremonyActConfiguration : IEntityTypeConfiguration<CeremonyAct>
{
    public void Configure(EntityTypeBuilder<CeremonyAct> builder)
    {
        builder.ToTable("ceremony_acts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.WeddingId).IsRequired();

        // Composite index supports GetByWeddingIdAsync ordered reads.
        // Covers the WHERE wedding_id = ? ORDER BY display_order, id pattern efficiently.
        builder.HasIndex(c => new { c.WeddingId, c.DisplayOrder, c.Id })
            .HasDatabaseName("ix_ceremony_acts_wedding_id_display_order");

        builder.Property(c => c.Name)
            .HasConversion(v => v.Value, v => ActivityName.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(ActivityName.MaxLength);

        builder.Property(c => c.Description)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? RichDescription.FromPersistence(v) : null)
            .HasMaxLength(RichDescription.MaxLength);

        builder.Property(c => c.Icon)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? IconIdentifier.FromPersistence(v) : null)
            .HasMaxLength(IconIdentifier.MaxLength);

        // TimeOnly? — maps to PostgreSQL "time without time zone".
        // Nullable: couples may describe acts without pinning them to a specific time.
        builder.Property(c => c.ActTime)
            .HasColumnType("time without time zone");

        builder.Property(c => c.DisplayOrder).IsRequired();
    }
}
