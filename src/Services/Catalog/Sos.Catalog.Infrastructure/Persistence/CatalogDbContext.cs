using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Catalog.Infrastructure.Persistence;

/// <summary>Catalog + Pricing + Inventory uchun birlashtirilgan DbContext.</summary>
public class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options,
    IMediator       mediator,
    ICurrentContext currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    // Catalog
    public DbSet<Product>         Products         => Set<Product>();
    public DbSet<Sku>             Skus             => Set<Sku>();
    public DbSet<Category>        Categories       => Set<Category>();
    public DbSet<MeasurementUnit> MeasurementUnits => Set<MeasurementUnit>();

    // Pricing
    public DbSet<PriceRule> PriceRules => Set<PriceRule>();

    // Inventory
    public DbSet<StockItem> StockItems => Set<StockItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
