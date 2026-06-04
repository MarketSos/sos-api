using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Analytics.Application.Interfaces;
using Sos.Analytics.Infrastructure.Persistence;
using Sos.Analytics.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.Analytics.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAnalyticsInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<AnalyticsDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ISaleSnapshotRepository, SaleSnapshotRepository>();

        return services;
    }
}
