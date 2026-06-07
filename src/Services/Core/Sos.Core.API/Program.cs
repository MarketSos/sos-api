using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Sos.Core.Application.Commands;
using Sos.Core.Domain.Entities.Identity;
using Sos.Core.Infrastructure.Extensions;
using Sos.Core.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Extensions;

if (OperatingSystem.IsWindows()) Console.Title = "Core.API";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
    .AddEnvironmentVariables();

builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:4200", "http://localhost:5500", "http://localhost:57859", "http://localhost:5000")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LoginCommand).Assembly));

builder.Services.AddCoreInfrastructure(builder.Configuration);

if (builder.Environment.IsDevelopment())
    builder.Services.AddSosSwaggerGen("Sos Core API", "http://localhost:5100", typeof(Program).Assembly);

builder.Services.AddHealthChecks().AddDbContextCheck<CoreDbContext>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSosExceptionHandling();
app.UseSerilogRequestLogging();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Logger.LogInformation("Sos.Core.API started in {Environment} mode", app.Environment.EnvironmentName);

using (var scope = app.Services.CreateScope())
{
    var db          = scope.ServiceProvider.GetRequiredService<CoreDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
    var logger      = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    await db.Database.MigrateAsync();
    await CoreDbContextSeed.SeedAsync(db, userManager, roleManager, logger);
}

app.Run();
