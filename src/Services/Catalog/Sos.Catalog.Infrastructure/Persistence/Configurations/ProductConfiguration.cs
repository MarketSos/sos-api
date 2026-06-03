using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);

        builder.HasIndex(p => p.Barcode).IsUnique();

        builder.Property(p => p.Barcode).HasMaxLength(100).IsRequired();
        builder.Property(p => p.NameUz).HasMaxLength(300).IsRequired();
        builder.Property(p => p.NameRu).HasMaxLength(300).IsRequired();
        builder.Property(p => p.NameEn).HasMaxLength(300);
        builder.Property(p => p.NameUzKiril).HasMaxLength(300);
        builder.Property(p => p.ImageUrl).HasMaxLength(500);

        builder.HasOne(p => p.Category)
               .WithMany(c => c.Products)
               .HasForeignKey(p => p.CategoryId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
