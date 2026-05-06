using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;

internal sealed class EntourageGroupConfiguration : IEntityTypeConfiguration<EntourageGroup>
{
    public void Configure(EntityTypeBuilder<EntourageGroup> builder)
    {
        builder.ToTable("entourage_groups");
        builder.HasKey(g => g.Id);

        // WeddingId — cross-aggregate reference to wedding.profiles.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(g => g.WeddingId).IsRequired();
        builder.HasIndex(g => g.WeddingId)
            .HasDatabaseName("ix_entourage_groups_wedding_id");

        builder.Property(g => g.Label)
            .IsRequired()
            .HasMaxLength(EntourageGroup.MaxLabelLength);

        builder.Property(g => g.Subtitle)
            .HasMaxLength(EntourageGroup.MaxSubtitleLength);

        // GroupType — nullable int column. C# enum default ordinal values.
        builder.Property(g => g.GroupType)
            .HasConversion<int?>();

        // RenderAs — non-nullable int column. C# enum default ordinal values.
        builder.Property(g => g.RenderAs)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(g => g.DisplayOrder).IsRequired();

        // ─── Members ─────────────────────────────────────────────────────────────
        builder.OwnsMany(g => g.Members, EntourageMemberConfiguration.Configure);

        builder.Navigation(g => g.Members)
            .HasField("_members")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        // ─── Couples ─────────────────────────────────────────────────────────────
        builder.OwnsMany(g => g.Couples, EntourageCoupleConfiguration.Configure);

        builder.Navigation(g => g.Couples)
            .HasField("_couples")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
