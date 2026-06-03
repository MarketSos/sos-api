using Microsoft.Extensions.DependencyInjection;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Shared.Infrastructure.Extensions;

/// <summary>
/// Общие инфраструктурные сервисы — подключаются во всех микросервисах.
/// </summary>
public static class SharedInfrastructureExtensions
{
    /// <summary>
    /// Регистрирует ICurrentUserService и IHttpContextAccessor.
    /// Вызывать в каждом Program.cs перед AddDbContext.
    /// </summary>
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        return services;
    }
}
