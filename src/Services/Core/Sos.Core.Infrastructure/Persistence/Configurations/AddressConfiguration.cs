using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.AddressLine).HasMaxLength(500).IsRequired();
        builder.Property(a => a.Region).HasMaxLength(100);
        builder.Property(a => a.District).HasMaxLength(100);
        builder.Property(a => a.Mahalla).HasMaxLength(200);
        builder.Property(a => a.Street).HasMaxLength(200);
        builder.Property(a => a.ReferencePoint).HasMaxLength(300);
    }
}
