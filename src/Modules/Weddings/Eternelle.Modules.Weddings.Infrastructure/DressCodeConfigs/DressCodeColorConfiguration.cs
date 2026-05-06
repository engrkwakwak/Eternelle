using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal sealed class DressCodeColorConfiguration
{
    public static void Configure(OwnedNavigationBuilder<DressCodeConfig, DressCodeColor> color)
    {
        // CHECK constraint mirrors the HexColor domain invariant: ^#[0-9A-Fa-f]{3,8}$
        // Using PostgreSQL regex match operator (~).
        color.ToTable("dress_code_colors", t => t.HasCheckConstraint(
            "ck_dress_code_colors_color_hex",
            "color_hex ~ '^#[0-9A-Fa-f]{3,8}$'"));

        color.HasKey(c => c.Id);

        // dress_code_config_id FK — cascade delete is the EF Core default for owned entities.
        color.WithOwner().HasForeignKey(c => c.DressCodeConfigId);

        // HexColor value object — stored as plain text; DB CHECK constraint enforces format.
        // HexColor.Create(v).Value is safe here: values written via the domain are always valid,
        // and the CHECK constraint prevents invalid data at the DB level.
        color.Property(c => c.ColorHex)
            .HasConversion(
                h => h.Value,
                v => HexColor.Create(v).Value)
            .IsRequired()
            .HasMaxLength(HexColor.MaxLength);

        color.Property(c => c.ColorName)
            .IsRequired()
            .HasMaxLength(DressCodeColor.MaxColorNameLength);

        color.Property(c => c.DisplayOrder).IsRequired();

        color.HasIndex(c => c.DressCodeConfigId)
            .HasDatabaseName("ix_dress_code_colors_dress_code_config_id");
    }
}
