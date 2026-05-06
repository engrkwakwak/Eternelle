using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Common.Infrastructure.Outbox;
using Eternelle.Common.Infrastructure.Inbox;
using Eternelle.Modules.Weddings.Domain.EntourageGroups;
using Eternelle.Modules.Weddings.Domain.Weddings;
using Eternelle.Modules.Weddings.Infrastructure.Database.Converters;
using Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;
using Eternelle.Modules.Weddings.Infrastructure.Weddings;
using Microsoft.EntityFrameworkCore;

namespace Eternelle.Modules.Weddings.Infrastructure.Database;

public sealed class WeddingsDbContext(DbContextOptions<WeddingsDbContext> options)
    : DbContext(options), IUnitOfWork
{
    internal DbSet<Wedding> Weddings { get; set; }
    internal DbSet<EntourageGroup> EntourageGroups { get; set; }

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
    }
}
