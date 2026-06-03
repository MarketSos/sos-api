using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;

namespace Sos.Catalog.Infrastructure.Persistence;

public class CatalogDbContext(DbContextOptions<CatalogDbContext> options, IMediator mediator)
    : BaseDbContext(options, mediator)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("catalog");

        builder.Entity<Product>(e =>
        {
            e.HasKey(p => p.Id);
            e.HasIndex(p => p.Barcode).IsUnique();
            e.Property(p => p.Name).HasMaxLength(300).IsRequired();
            e.Property(p => p.BasePrice).HasColumnType("decimal(18,2)");
            e.Property(p => p.CostPrice).HasColumnType("decimal(18,2)");
        });

        builder.Entity<Category>(e =>
        {
            e.HasKey(c => c.Id);
            e.HasMany(c => c.Children).WithOne(c => c.Parent).HasForeignKey(c => c.ParentId);
            e.HasMany(c => c.Products).WithOne().HasForeignKey(p => p.CategoryId);
        });
    }
}
