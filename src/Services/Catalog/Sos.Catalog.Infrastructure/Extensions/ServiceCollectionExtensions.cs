using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Catalog.Application.Commands;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Infrastructure.Messaging;
using Sos.Catalog.Infrastructure.Persistence;
using Sos.Catalog.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;
using Sos.Shared.Infrastructure.Messaging;

namespace Sos.Catalog.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();
        services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);

        services.AddDbContext<CatalogDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Catalog repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ISkuRepository, SkuRepository>();
        services.AddScoped<IMeasurementUnitRepository, MeasurementUnitRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();

        // Pricing repositories
        services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();

        // Inventory repositories
        services.AddScoped<IStockRepository, StockRepository>();

        // MassTransit + RabbitMQ — SaleCompleted hodisasini eshitish
        services.AddSosMassTransit(config, x =>
        {
            x.AddConsumer<SaleCompletedConsumer>();
        });

        return services;
    }
}
