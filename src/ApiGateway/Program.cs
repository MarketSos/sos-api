using Serilog;

if (OperatingSystem.IsWindows()) Console.Title = "Sos.ApiGateway";

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200", "http://localhost:57859")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.Authority = builder.Configuration["IdentityService:Authority"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();
builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseCors();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/identity/v1/swagger.json",   "Identity API");
    c.SwaggerEndpoint("/swagger/catalog/v1/swagger.json",    "Catalog API");
    c.SwaggerEndpoint("/swagger/inventory/v1/swagger.json",  "Inventory API");
    c.SwaggerEndpoint("/swagger/pos/v1/swagger.json",        "POS API");
    c.SwaggerEndpoint("/swagger/pricing/v1/swagger.json",    "Pricing API");
    c.SwaggerEndpoint("/swagger/crm/v1/swagger.json",        "CRM API");
    c.SwaggerEndpoint("/swagger/loyalty/v1/swagger.json",    "Loyalty API");
    c.SwaggerEndpoint("/swagger/analytics/v1/swagger.json",  "Analytics API");
    c.RoutePrefix = "swagger";
});

app.UseAuthentication();
app.UseAuthorization();
app.MapReverseProxy();
app.MapHealthChecks("/health");

app.Run();
