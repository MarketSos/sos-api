using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class SkuConfiguration : IEntityTypeConfiguration<Sku>
{
    public void Configure(EntityTypeBuilder<Sku> builder)
    {
        builder.HasKey(s => s.Id);

        builder.HasIndex(s => s.SerialNumber).IsUnique();

        builder.Property(s => s.SerialNumber).HasMaxLength(100).IsRequired();
        builder.Property(s => s.Amount).HasColumnType("decimal(18,3)");
        builder.Property(s => s.Weight).HasColumnType("decimal(10,3)");
        builder.Property(s => s.CostPrice).HasColumnType("decimal(18,2)");
        builder.Property(s => s.SalePrice).HasColumnType("decimal(18,2)");
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);

        builder.HasOne(s => s.Product)
               .WithMany()
               .HasForeignKey(s => s.ProductId)
               .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.MeasurementUnit)
               .WithMany()
               .HasForeignKey(s => s.MeasurementUnitId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
