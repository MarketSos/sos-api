using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.CRM.Application.Interfaces;
using Sos.CRM.Infrastructure.Persistence;
using Sos.CRM.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;

namespace Sos.CRM.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCrmInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();

        services.AddDbContext<CrmDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        services.AddScoped<ICustomerRepository, CustomerRepository>();

        return services;
    }
}
