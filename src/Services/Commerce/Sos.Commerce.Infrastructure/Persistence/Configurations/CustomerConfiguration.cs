using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Commerce.Domain.Entities;

namespace Sos.Commerce.Infrastructure.Persistence.Configurations;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.LastName).IsRequired().HasMaxLength(100);
        builder.Property(x => x.PhoneNumber).HasMaxLength(20);
        builder.Property(x => x.Email).HasMaxLength(200);
        builder.HasIndex(x => x.PhoneNumber).IsUnique().HasFilter("\"PhoneNumber\" IS NOT NULL");
        builder.HasIndex(x => x.Email).HasFilter("\"Email\" IS NOT NULL");
        builder.Ignore(x => x.FullName);
    }
}
