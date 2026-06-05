using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Catalog.Infrastructure.Persistence;

public class CatalogDbContext(
    DbContextOptions<CatalogDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<Product>         Products         => Set<Product>();
    public DbSet<Sku>             Skus             => Set<Sku>();
    public DbSet<Category>        Categories       => Set<Category>();
    public DbSet<MeasurementUnit> MeasurementUnits => Set<MeasurementUnit>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configurations/ papkasidagi barcha IEntityTypeConfiguration<T> larni avtomatik yuklaydi
        builder.ApplyConfigurationsFromAssembly(typeof(CatalogDbContext).Assembly);
    }
}
