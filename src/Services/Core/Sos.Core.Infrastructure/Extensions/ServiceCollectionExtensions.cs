using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Sos.Core.Application.Commands;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;
using Sos.Core.Infrastructure.Repositories;
using Sos.Core.Infrastructure.Services;
using Sos.Shared.Infrastructure.Extensions;
using System.Text;

namespace Sos.Core.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCoreInfrastructure(
        this IServiceCollection services,
        IConfiguration config)
    {
        services.AddSharedInfrastructure();
        services.AddValidatorsFromAssembly(typeof(LoginCommand).Assembly);

        services.AddDbContext<CoreDbContext>(opts =>
            opts.UseNpgsql(config.GetConnectionString("Default")));

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IOrganizationRepository, OrganizationRepository>();

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
