using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Analytics.Domain.Entities;

namespace Sos.Analytics.Infrastructure.Persistence.Configurations;

public class SaleSnapshotConfiguration : IEntityTypeConfiguration<SaleSnapshot>
{
    public void Configure(EntityTypeBuilder<SaleSnapshot> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.SaleId).IsUnique();
        builder.HasIndex(x => new { x.StoreId, x.CompletedAt });
        builder.Property(x => x.TotalAmount).HasPrecision(18, 4);
    }
}
