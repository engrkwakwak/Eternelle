using Eternelle.Modules.Weddings.Domain.CeremonyActs;
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
        builder.HasIndex(c => c.WeddingId)
            .HasDatabaseName("ix_ceremony_acts_wedding_id");

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(CeremonyAct.MaxNameLength);

        builder.Property(c => c.Description)
            .HasMaxLength(CeremonyAct.MaxDescriptionLength);

        builder.Property(c => c.Icon)
            .HasMaxLength(CeremonyAct.MaxIconLength);

        // TimeOnly? — maps to PostgreSQL "time without time zone".
        // Nullable: couples may describe acts without pinning them to a specific time.
        builder.Property(c => c.ActTime)
            .HasColumnType("time without time zone");

        builder.Property(c => c.DisplayOrder).IsRequired();
    }
}
