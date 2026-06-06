using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Analytics.Application.Commands;
using Sos.Analytics.Application.Interfaces;
using Sos.Analytics.Infrastructure.Messaging;
using Sos.Analytics.Infrastructure.Persistence;
using Sos.Analytics.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;
using Sos.Shared.Infrastructure.Messaging;

namespace Sos.Analytics.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();
        services.AddValidatorsFromAssembly(typeof(RecordSaleCommand).Assembly);

        services.AddDbContext<AnalyticsDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ISaleSnapshotRepository, SaleSnapshotRepository>();

        // MassTransit + RabbitMQ — SaleCompleted hodisasini eshitish
        services.AddSosMassTransit(config, x =>
        {
            x.AddConsumer<SaleCompletedConsumer>();
        });

        return services;
    }
}
