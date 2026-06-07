using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class ManufacturerConfiguration : IEntityTypeConfiguration<Manufacturer>
{
    public void Configure(EntityTypeBuilder<Manufacturer> builder)
    {
        builder.HasKey(m => m.Id);

        builder.Property(m => m.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(m => m.Code).IsUnique();

        builder.Property(m => m.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(m => m.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(m => m.NameEn).HasMaxLength(200);
        builder.Property(m => m.NameUzKiril).HasMaxLength(200);
    }
}
