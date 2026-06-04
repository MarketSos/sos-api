using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Pricing.Application.Interfaces;
using Sos.Pricing.Infrastructure.Persistence;
using Sos.Pricing.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.Pricing.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddPricingInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<PricingDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<IPriceRuleRepository, PriceRuleRepository>();

        return services;
    }
}
