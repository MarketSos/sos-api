using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sos.Identity.Application.Commands;
using Sos.Identity.Application.Interfaces;
using Sos.Identity.Domain.Entities;
using Sos.Identity.Infrastructure.Persistence;
using Sos.Identity.Infrastructure.Repositories;
using Sos.Identity.Infrastructure.Services;
using Sos.Shared.Infrastructure.Extensions;
using System.Text;

namespace Sos.Identity.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddIdentityInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        // Shared: ICurrentUserService, IHttpContextAccessor, ValidationBehavior
        services.AddSharedInfrastructure();

        // FluentValidation — Application assembly-dan barcha validatorlarni ro'yxatdan o'tkazish
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

        // Database
        services.AddDbContext<IdentityDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // Services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

        // JWT Authentication
        var jwtSecret = config["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not configured");
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(opts =>
            {
                opts.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer           = true,
                    ValidateAudience         = true,
                    ValidateLifetime         = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer              = config["Jwt:Issuer"],
                    ValidAudience            = config["Jwt:Audience"],
                    IssuerSigningKey         = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                    ClockSkew                = TimeSpan.Zero
                };
            });

        services.AddAuthorization();

        return services;
    }
}
