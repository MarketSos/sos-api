using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Inventory.Application.Interfaces;
using Sos.Inventory.Infrastructure.Persistence;
using Sos.Inventory.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.Inventory.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInventoryInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<InventoryDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IStockRepository, StockRepository>();

        return services;
    }
}
