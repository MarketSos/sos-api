using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(b => b.Id);

        builder.Property(b => b.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(b => b.Code).IsUnique();

        builder.Property(b => b.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(b => b.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(b => b.NameEn).HasMaxLength(200);
        builder.Property(b => b.NameUzKiril).HasMaxLength(200);
    }
}
