using Eternelle.Common.Application.EventBus;
using Eternelle.Common.Application.Messaging;
using Eternelle.Common.Infrastructure.Outbox;
using Eternelle.Common.Presentation.Endpoints;
using Eternelle.Modules.Weddings.Application.Abstractions.Data;
using Eternelle.Modules.Weddings.Application.Abstractions.Subscriptions;
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
using Eternelle.Modules.Weddings.Infrastructure.Database;
using Eternelle.Modules.Weddings.Infrastructure.DressCodeConfigs;
using Eternelle.Modules.Weddings.Infrastructure.EntourageGroups;
using Eternelle.Modules.Weddings.Infrastructure.GalleryImages;
using Eternelle.Modules.Weddings.Infrastructure.GiftOptions;
using Eternelle.Modules.Weddings.Infrastructure.GuestPhotos;
using Eternelle.Modules.Weddings.Infrastructure.Inbox;
using Eternelle.Modules.Weddings.Infrastructure.Outbox;
using Eternelle.Modules.Weddings.Infrastructure.Reminders;
using Eternelle.Modules.Weddings.Infrastructure.StoryMoments;
using Eternelle.Modules.Weddings.Infrastructure.Subscriptions;
using Eternelle.Modules.Weddings.Infrastructure.VendorCredits;
using Eternelle.Modules.Weddings.Infrastructure.Weddings;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Eternelle.Modules.Weddings.Infrastructure;

public static class WeddingsModule
{
    public static IServiceCollection AddWeddingsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDomainEventHandlers();

        services.AddIntegrationEventHandlers();

        services.AddInfrastructure(configuration);

        services.AddEndpoints(Presentation.AssemblyReference.Assembly);

        return services;
    }

    private static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<WeddingsDbContext>((sp, options) =>
            options
                .UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsqlOptions => npgsqlOptions
                        .MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemas.Weddings))
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(sp.GetRequiredService<InsertOutboxMessagesInterceptor>()));

        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<WeddingsDbContext>());

        services.AddScoped<ISubscriptionPlanService, StubSubscriptionPlanService>();

        services.AddScoped<IWeddingRepository, WeddingRepository>();
        services.AddScoped<IEntourageGroupRepository, EntourageGroupRepository>();
        services.AddScoped<IStoryMomentRepository, StoryMomentRepository>();
        services.AddScoped<IGalleryImageRepository, GalleryImageRepository>();
        services.AddScoped<IGiftOptionRepository, GiftOptionRepository>();
        services.AddScoped<IDressCodeConfigRepository, DressCodeConfigRepository>();
        services.AddScoped<IGuestPhotoRepository, GuestPhotoRepository>();
        services.AddScoped<ICeremonyActRepository, CeremonyActRepository>();
        services.AddScoped<IVendorCreditRepository, VendorCreditRepository>();
        services.AddScoped<IReminderRepository, ReminderRepository>();

        services.Configure<OutboxOptions>(configuration.GetSection("Weddings:Outbox"));

        services.ConfigureOptions<ConfigureProcessOutboxJob>();

        services.Configure<InboxOptions>(configuration.GetSection("Weddings:Inbox"));

        services.ConfigureOptions<ConfigureProcessInboxJob>();
    }

    private static void AddDomainEventHandlers(this IServiceCollection services)
    {
        Type[] domainEventHandlers = Application.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IDomainEventHandler)))
            .ToArray();

        foreach (Type domainEventHandler in domainEventHandlers)
        {
            services.TryAddScoped(domainEventHandler);

            Type domainEvent = domainEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler = typeof(IdempotentDomainEventHandler<>).MakeGenericType(domainEvent);

            services.Decorate(domainEventHandler, closedIdempotentHandler);
        }
    }

    private static void AddIntegrationEventHandlers(this IServiceCollection services)
    {
        Type[] integrationEventHandlers = Presentation.AssemblyReference.Assembly
            .GetTypes()
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEventHandler)))
            .ToArray();

        foreach (Type integrationEventHandler in integrationEventHandlers)
        {
            services.TryAddScoped(integrationEventHandler);

            Type integrationEvent = integrationEventHandler
                .GetInterfaces()
                .Single(i => i.IsGenericType)
                .GetGenericArguments()
                .Single();

            Type closedIdempotentHandler =
                typeof(IdempotentIntegrationEventHandler<>).MakeGenericType(integrationEvent);

            services.Decorate(integrationEventHandler, closedIdempotentHandler);
        }
    }
}
