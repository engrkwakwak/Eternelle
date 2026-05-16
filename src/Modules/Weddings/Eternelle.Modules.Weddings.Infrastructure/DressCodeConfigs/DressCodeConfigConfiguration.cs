using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal sealed class DressCodeConfigConfiguration : IEntityTypeConfiguration<DressCodeConfig>
{
    public void Configure(EntityTypeBuilder<DressCodeConfig> builder)
    {
        builder.ToTable("dress_code_configs");
        builder.HasKey(d => d.Id);

        // WeddingId — cross-aggregate reference. 1:1 with wedding.profiles enforced by UNIQUE index.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(d => d.WeddingId).IsRequired();
        builder.HasIndex(d => d.WeddingId)
            .IsUnique()
            .HasDatabaseName("ix_dress_code_configs_wedding_id");

        builder.Property(d => d.Description)
            .HasConversion(v => v.Value, v => RichDescription.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(RichDescription.MaxLength);

        // ─── Colors ──────────────────────────────────────────────────────────────
        builder.OwnsMany(d => d.Colors, DressCodeColorConfiguration.Configure);

        builder.Navigation(d => d.Colors)
            .HasField("_colors")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Images ──────────────────────────────────────────────────────────────
        builder.OwnsMany(d => d.Images, DressCodeImageConfiguration.Configure);

        builder.Navigation(d => d.Images)
            .HasField("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
