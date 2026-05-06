using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;

internal static class DressCodeImageConfiguration
{
    public static void Configure(OwnedNavigationBuilder<DressCodeConfig, DressCodeImage> image)
    {
        image.ToTable("dress_code_images");

        image.HasKey(i => i.Id);

        image.WithOwner().HasForeignKey(i => i.DressCodeConfigId);

        image.Property(i => i.ImageUrl).IsRequired();

        image.Property(i => i.DisplayOrder).IsRequired();
    }
}
