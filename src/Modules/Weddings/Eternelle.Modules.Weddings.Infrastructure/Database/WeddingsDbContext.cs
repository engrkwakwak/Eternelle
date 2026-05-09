using Eternelle.Common.Infrastructure.Inbox;
using Eternelle.Common.Infrastructure.Outbox;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Domain.CeremonyActs;
using Eternelle.Modules.Weddings.Domain.DressCodeConfigs;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.GalleryImages;
using Eternelle.Modules.Weddings.Domain.GiftOptions;
using Eternelle.Modules.Weddings.Domain.GuestPhotos;
using Eternelle.Modules.Weddings.Domain.Reminders;
using Eternelle.Modules.Weddings.Domain.StoryMoments;
using Eternelle.Modules.Weddings.Domain.VendorCredits;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.CeremonyActs;
using Eternelle.Modules.Weddings.Infrastructure.Database.Converters;
using Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;
using Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;
using Eternelle.Modules.Weddings.Infrastructure.GalleryImages;
using Eternelle.Modules.Weddings.Infrastructure.GiftOptions;
using Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;
using Eternelle.Modules.Weddings.Infrastructure.Reminders;
using Eternelle.Modules.Weddings.Infrastructure.StoryMoments;
using Eternelle.Modules.Weddings.Infrastructure.VendorCredits;
using Eternelle.Modules.Weddings.Infrastructure.Weddings;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.Database;

public sealed class WeddingsDbContext(DbContextOptions<WeddingsDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Wedding> Weddings { get; set; }
    internal DbSet<EntourageGroup> EntourageGroups { get; set; }
    internal DbSet<StoryMoment> StoryMoments { get; set; }
    internal DbSet<GalleryImage> GalleryImages { get; set; }
    internal DbSet<GiftOption> GiftOptions { get; set; }
    internal DbSet<DressCodeConfig> DressCodeConfigs { get; set; }
    internal DbSet<GuestPhoto> GuestPhotos { get; set; }
    internal DbSet<CeremonyAct> CeremonyActs { get; set; }
    internal DbSet<VendorCredit> VendorCredits { get; set; }
    internal DbSet<Reminder> Reminders { get; set; }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<WeddingId>()
            .HaveConversion<WeddingIdConverter>();

        configurationBuilder.Properties<PartnerId>()
            .HaveConversion<PartnerIdConverter>();

        configurationBuilder.Properties<SnapShareConfigId>()
            .HaveConversion<SnapShareConfigIdConverter>();

        configurationBuilder.Properties<EntourageGroupId>()
            .HaveConversion<EntourageGroupIdConverter>();

        configurationBuilder.Properties<EntourageMemberId>()
            .HaveConversion<EntourageMemberIdConverter>();

        configurationBuilder.Properties<EntourageCoupleId>()
            .HaveConversion<EntourageCoupleIdConverter>();

        configurationBuilder.Properties<StoryMomentId>()
            .HaveConversion<StoryMomentIdConverter>();

        configurationBuilder.Properties<GalleryImageId>()
            .HaveConversion<GalleryImageIdConverter>();

        configurationBuilder.Properties<GiftOptionId>()
            .HaveConversion<GiftOptionIdConverter>();

        configurationBuilder.Properties<DressCodeConfigId>()
            .HaveConversion<DressCodeConfigIdConverter>();

        configurationBuilder.Properties<DressCodeColorId>()
            .HaveConversion<DressCodeColorIdConverter>();

        configurationBuilder.Properties<DressCodeImageId>()
            .HaveConversion<DressCodeImageIdConverter>();

        configurationBuilder.Properties<GuestPhotoId>()
            .HaveConversion<GuestPhotoIdConverter>();

        configurationBuilder.Properties<CeremonyActId>()
            .HaveConversion<CeremonyActIdConverter>();

        configurationBuilder.Properties<VendorCreditId>()
            .HaveConversion<VendorCreditIdConverter>();

        configurationBuilder.Properties<ReminderId>()
            .HaveConversion<ReminderIdConverter>();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(Schemas.Weddings);

        modelBuilder.ApplyConfiguration(new OutboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new OutboxMessageConsumerConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConfiguration());
        modelBuilder.ApplyConfiguration(new InboxMessageConsumerConfiguration());

        modelBuilder.ApplyConfiguration(new WeddingConfiguration());
        modelBuilder.ApplyConfiguration(new EntourageGroupConfiguration());
        modelBuilder.ApplyConfiguration(new StoryMomentConfiguration());
        modelBuilder.ApplyConfiguration(new GalleryImageConfiguration());
        modelBuilder.ApplyConfiguration(new GiftOptionConfiguration());
        modelBuilder.ApplyConfiguration(new DressCodeConfigConfiguration());
        modelBuilder.ApplyConfiguration(new GuestPhotoConfiguration());
        modelBuilder.ApplyConfiguration(new CeremonyActConfiguration());
        modelBuilder.ApplyConfiguration(new VendorCreditConfiguration());
        modelBuilder.ApplyConfiguration(new ReminderConfiguration());
    }
}
