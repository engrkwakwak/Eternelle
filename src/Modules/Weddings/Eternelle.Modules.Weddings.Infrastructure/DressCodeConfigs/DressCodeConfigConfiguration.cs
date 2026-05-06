using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal sealed class DressCodeConfigConfiguration : IEntityTypeConfiguration<DressCodeConfig>
{
    public void Configure(EntityTypeBuilder<DressCodeConfig> builder)
    {
        builder.ToTable("dress_code_configs");

        builder.HasKey(d => d.Id);

        // 1:1 with wedding.profiles — enforced by the UNIQUE index on wedding_id.
        builder.Property(d => d.WeddingId).IsRequired();
        builder.HasIndex(d => d.WeddingId).IsUnique();

        builder.Property(d => d.Description)
            .IsRequired()
            .HasMaxLength(DressCodeConfig.MaxDescriptionLength);

        // ─── Colors ──────────────────────────────────────────────────────────────
        // Owned collection in its own table (wedding.dress_code_colors).
        // EF Core uses the _colors backing field to load and track the collection.

        builder.OwnsMany(d => d.Colors, DressCodeColorConfiguration.Configure);

        builder.Navigation(d => d.Colors)
            .HasField("_colors")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Images ──────────────────────────────────────────────────────────────
        // Owned collection in its own table (wedding.dress_code_images).
        // EF Core uses the _images backing field to load and track the collection.

        builder.OwnsMany(d => d.Images, DressCodeImageConfiguration.Configure);

        builder.Navigation(d => d.Images)
            .HasField("_images")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
