using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Infrastructure.Persistence;
using Sos.Catalog.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.Catalog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISkuRepository, SkuRepository>();
        services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();

        return services;
    }
}
