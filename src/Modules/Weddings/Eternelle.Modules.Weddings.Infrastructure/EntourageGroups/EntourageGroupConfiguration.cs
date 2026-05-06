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

        // GroupType — nullable, stored as snake_case string to match the PostgreSQL ENUM.
        builder.Property(g => g.GroupType)
            .HasConversion(
                v => v.HasValue ? GroupTypeToString(v.Value) : null,
                v => v != null ? StringToGroupType(v) : (EntourageGroupType?)null)
            .HasColumnName("group_type");

        // RenderAs — non-nullable, stored as snake_case string to match the PostgreSQL ENUM.
        builder.Property(g => g.RenderAs)
            .HasConversion(
                v => RenderModeToString(v),
                v => StringToRenderMode(v))
            .HasColumnName("render_as")
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

    // ─── Enum ↔ string helpers ───────────────────────────────────────────────────

    private static string GroupTypeToString(EntourageGroupType value) => value switch
    {
        EntourageGroupType.Parents            => "parents",
        EntourageGroupType.PrincipalSponsors  => "principal_sponsors",
        EntourageGroupType.SecondarySponsors  => "secondary_sponsors",
        EntourageGroupType.Bridesmaids        => "bridesmaids",
        EntourageGroupType.Groomsmen          => "groomsmen",
        EntourageGroupType.FlowerGirls        => "flower_girls",
        EntourageGroupType.RingBearers        => "ring_bearers",
        EntourageGroupType.CoinBearers        => "coin_bearers",
        EntourageGroupType.BibleReaders       => "bible_readers",
        EntourageGroupType.LittleOnes         => "little_ones",
        EntourageGroupType.Other              => "other",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static EntourageGroupType StringToGroupType(string value) => value switch
    {
        "parents"             => EntourageGroupType.Parents,
        "principal_sponsors"  => EntourageGroupType.PrincipalSponsors,
        "secondary_sponsors"  => EntourageGroupType.SecondarySponsors,
        "bridesmaids"         => EntourageGroupType.Bridesmaids,
        "groomsmen"           => EntourageGroupType.Groomsmen,
        "flower_girls"        => EntourageGroupType.FlowerGirls,
        "ring_bearers"        => EntourageGroupType.RingBearers,
        "coin_bearers"        => EntourageGroupType.CoinBearers,
        "bible_readers"       => EntourageGroupType.BibleReaders,
        "little_ones"         => EntourageGroupType.LittleOnes,
        "other"               => EntourageGroupType.Other,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static string RenderModeToString(EntourageRenderMode value) => value switch
    {
        EntourageRenderMode.Cards => "cards",
        EntourageRenderMode.List  => "list",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };

    private static EntourageRenderMode StringToRenderMode(string value) => value switch
    {
        "cards" => EntourageRenderMode.Cards,
        "list"  => EntourageRenderMode.List,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
    };
}
