using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Sos.Shared.Infrastructure.Messaging;

/// <summary>
/// Barcha servislar uchun umumiy MassTransit + RabbitMQ konfiguratsiyasi.
/// Общая конфигурация MassTransit + RabbitMQ для всех сервисов.
/// </summary>
public static class MassTransitExtensions
{
    /// <summary>
    /// MassTransit ni RabbitMQ bilan ro'yxatdan o'tkazadi.
    /// Регистрирует MassTransit с RabbitMQ.
    /// </summary>
    /// <param name="addConsumers">
    /// Consumer'larni qo'shish uchun callback.
    /// Callback для добавления consumer'ов.
    /// </param>
    public static IServiceCollection AddSosMassTransit(
        this IServiceCollection services,
        IConfiguration config,
        Action<IBusRegistrationConfigurator>? addConsumers = null)
    {
        services.AddMassTransit(x =>
        {
            // Consumer'larni ro'yxatdan o'tkazish / Регистрация consumer'ов
            addConsumers?.Invoke(x);

            x.UsingRabbitMq((ctx, cfg) =>
            {
                var host     = config["RabbitMQ:Host"]        ?? "localhost";
                var user     = config["RabbitMQ:Username"]    ?? "guest";
                var pass     = config["RabbitMQ:Password"]    ?? "guest";
                var vhost    = config["RabbitMQ:VirtualHost"] ?? "/";
                var port     = ushort.Parse(config["RabbitMQ:Port"] ?? "5672");

                cfg.Host(host, port, vhost, h =>
                {
                    h.Username(user);
                    h.Password(pass);
                });

                // Prefetch va retry sozlamalari / Настройки prefetch и повтора
                cfg.PrefetchCount             = 16;
                cfg.ConcurrentMessageLimit    = 8;

                // Avtomatik retry: 3 ta, 5 sekunddan / Автоповтор: 3 раза с 5 сек
                cfg.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));

                // Queue va exchange nomlari avtomatik generated / Имена очередей автоматические
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }
}
