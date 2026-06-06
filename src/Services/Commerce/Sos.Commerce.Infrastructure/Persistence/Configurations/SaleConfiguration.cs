using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.Infrastructure.Persistence.Configurations;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.SubTotal).HasColumnType("decimal(18,2)");
        builder.Property(s => s.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.TaxAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.PaidAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.ChangeAmount).HasColumnType("decimal(18,2)");
        builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.PaymentMethod).HasConversion<string>().HasMaxLength(20);
        builder.Property(s => s.ReceiptNumber).HasMaxLength(100);

        builder.HasMany(s => s.Items)
               .WithOne()
               .HasForeignKey(i => i.SaleId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.HasKey(i => i.Id);
        builder.Property(i => i.ProductName).HasMaxLength(300).IsRequired();
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)");
        builder.Ignore(i => i.TotalPrice);
    }
}
