using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.POS.Application.Interfaces;
using Sos.POS.Infrastructure.Persistence;
using Sos.POS.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.POS.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPosInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<PosDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ISaleRepository, SaleRepository>();

        return services;
    }
}
