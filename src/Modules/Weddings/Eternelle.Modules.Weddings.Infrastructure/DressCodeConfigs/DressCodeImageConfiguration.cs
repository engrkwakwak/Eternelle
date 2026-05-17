using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.Shared;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal sealed class DressCodeImageConfiguration
{
    public static void Configure(OwnedNavigationBuilder<DressCodeConfig, DressCodeImage> image)
    {
        image.ToTable("dress_code_images");
        image.HasKey(i => i.Id);

        // dress_code_config_id FK — cascade delete is the EF Core default for owned entities.
        image.WithOwner().HasForeignKey(i => i.DressCodeConfigId);

        image.Property(i => i.ImageUrl)
            .HasConversion(v => v.Value, v => ImageUrl.FromPersistence(v))
            .IsRequired()
            .HasMaxLength(ImageUrl.MaxLength);
        image.Property(i => i.DisplayOrder).IsRequired();

        // Composite index supports ordered loads via Include on DressCodeConfigRepository.
        // Covers the WHERE dress_code_config_id = ? ORDER BY display_order, id pattern efficiently.
        image.HasIndex(i => new { i.DressCodeConfigId, i.DisplayOrder, i.Id })
            .HasDatabaseName("ix_dress_code_images_dress_code_config_id_display_order");
    }
}
