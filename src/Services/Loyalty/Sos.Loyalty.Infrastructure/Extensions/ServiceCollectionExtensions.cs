using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Loyalty.Application.Interfaces;
using Sos.Loyalty.Infrastructure.Persistence;
using Sos.Loyalty.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.Loyalty.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLoyaltyInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<LoyaltyDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ILoyaltyRepository, LoyaltyRepository>();

        return services;
    }
}
