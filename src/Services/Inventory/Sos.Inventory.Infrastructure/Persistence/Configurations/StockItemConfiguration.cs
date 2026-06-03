using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Inventory.Domain.Entities;

namespace Sos.Inventory.Infrastructure.Persistence.Configurations;

public class StockItemConfiguration : IEntityTypeConfiguration<StockItem>
{
    public void Configure(EntityTypeBuilder<StockItem> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => new { s.ProductId, s.StoreId }).IsUnique();

        builder.Property(s => s.Location).HasMaxLength(100);
    }
}
