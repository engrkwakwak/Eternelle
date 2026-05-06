using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal static class DressCodeColorConfiguration
{
    public static void Configure(OwnedNavigationBuilder<DressCodeConfig, DressCodeColor> color)
    {
        color.ToTable("dress_code_colors", t => t.HasCheckConstraint(
            "ck_dress_code_colors_color_hex",
            "color_hex ~ '^#[0-9A-Fa-f]{3,8}$'"));

        color.HasKey(c => c.Id);

        color.WithOwner().HasForeignKey(c => c.DressCodeConfigId);

        // HexColor — stored as raw text; the CHECK constraint above enforces the
        // regex on the DB side. HexColor.Create().Value is safe here because only
        // values that passed CHECK can reach the read path.
        color.Property(c => c.ColorHex)
            .HasConversion(
                h => h.Value,
                v => HexColor.Create(v).Value)
            .HasColumnName("color_hex")
            .IsRequired()
            .HasMaxLength(HexColor.MaxLength);

        color.Property(c => c.ColorName)
            .IsRequired()
            .HasMaxLength(DressCodeColor.MaxColorNameLength);

        color.Property(c => c.DisplayOrder).IsRequired();
    }
}
