using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.HasKey(s => s.Id);
        builder.HasIndex(s => new { s.ProductId, s.StoreId }).IsUnique();
        builder.Property(s => s.Location).HasMaxLength(100);
    }
}
