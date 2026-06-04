using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sos.Shared.Infrastructure.Behaviors;
using Sos.Shared.Infrastructure.Middleware;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Shared.Infrastructure.Extensions;

/// <summary>
/// Umumiy infrastruktura servislar — barcha mikroservislarda ishlatiladi.
/// </summary>
public static class SharedInfrastructureExtensions
{
    /// <summary>
    /// ICurrentUserService, IHttpContextAccessor va ValidationBehavior ro'yxatdan o'tkazadi.
    /// Har bir servisning Infrastructure extension-ida chaqiriladi.
    /// </summary>
    public static IServiceCollection AddSharedInfrastructure(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ICurrentUserService, CurrentUserService>();

        // MediatR pipeline: har bir request-dan oldin FluentValidation ishga tushadi.
        // Agar validator topilmasa — behavior o'tkazib yuboradi (no-op).
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        return services;
    }

    /// <summary>
    /// Global exception handler middleware-ni pipeline-ga qo'shadi.
    /// app.UseSosExceptionHandling() ni UseAuthentication/UseAuthorization-dan OLDIN chaqiring.
    /// </summary>
    public static IApplicationBuilder UseSosExceptionHandling(this IApplicationBuilder app)
        => app.UseMiddleware<ExceptionHandlingMiddleware>();
}
