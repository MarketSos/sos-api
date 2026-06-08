using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class StoreConfiguration : IEntityTypeConfiguration<Store>
{
    public void Configure(EntityTypeBuilder<Store> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => new { s.OrganizationId, s.Code }).IsUnique();

        builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
        builder.Property(s => s.Address).HasMaxLength(500);
        builder.Property(s => s.Phone).HasMaxLength(50);

        // StockItems bilan bog'lanish
        builder.HasMany(s => s.StockItems)
               .WithOne(si => si.Store)
               .HasForeignKey(si => si.StoreId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
