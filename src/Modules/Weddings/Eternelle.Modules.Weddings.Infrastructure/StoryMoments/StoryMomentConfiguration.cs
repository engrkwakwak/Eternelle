using Eternelle.Modules.Weddings.Domain.Shared;
using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.StoryMoments;

internal sealed class StoryMomentConfiguration : IEntityTypeConfiguration<StoryMoment>
{
    public void Configure(EntityTypeBuilder<StoryMoment> builder)
    {
        builder.ToTable("story_moments");
        builder.HasKey(s => s.Id);

        // WeddingId — cross-aggregate reference.
        // Converter registered globally in WeddingsDbContext.ConfigureConventions.
        builder.Property(s => s.WeddingId).IsRequired();
        builder.HasIndex(s => new { s.WeddingId, s.DisplayOrder, s.Id })
            .HasDatabaseName("ix_story_moments_wedding_id_display_order_id");

        builder.Property(s => s.Title)
            .HasConversion(v => v.Value, v => ActivityName.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(ActivityName.MaxLength);

        builder.Property(s => s.StoryDate);

        builder.Property(s => s.Description)
            .HasConversion(v => v.Value, v => RichDescription.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(RichDescription.MaxLength);

        builder.Property(s => s.ImageUrl)
            .HasConversion(
                v => v != null ? v.Value : null,
                v => v != null ? ImageUrl.FromPersistence(v) : null)
            .HasMaxLength(ImageUrl.MaxLength);

        builder.Property(s => s.DisplayOrder).IsRequired();
    }
}
