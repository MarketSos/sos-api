using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sos.Commerce.Application.Commands;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Infrastructure.Messaging;
using Sos.Commerce.Infrastructure.Persistence;
using Sos.Commerce.Infrastructure.Repositories;
using Sos.Shared.Infrastructure.Extensions;
using Sos.Shared.Infrastructure.Messaging;

namespace Sos.Commerce.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCommerceInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();
        services.AddValidatorsFromAssembly(typeof(CreateSaleCommand).Assembly);

        services.AddDbContext<CommerceDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // POS
        services.AddScoped<ISaleRepository, SaleRepository>();

        // Loyalty
        services.AddScoped<ILoyaltyRepository, LoyaltyRepository>();

        // CRM
        services.AddScoped<ICustomerRepository, CustomerRepository>();

        // Messaging — publish
        services.AddScoped<ISaleEventPublisher, SaleEventPublisher>();

        // MassTransit + RabbitMQ (Commerce faqat publish qiladi, consumer yo'q)
        services.AddSosMassTransit(config);

        return services;
    }
}
