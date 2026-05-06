using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
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

        image.Property(i => i.ImageUrl).IsRequired();
        image.Property(i => i.DisplayOrder).IsRequired();

        image.HasIndex(i => i.DressCodeConfigId)
            .HasDatabaseName("ix_dress_code_images_dress_code_config_id");
    }
}
