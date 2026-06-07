using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class OrgTypeConfiguration : IEntityTypeConfiguration<OrgType>
{
    public void Configure(EntityTypeBuilder<OrgType> builder)
    {
        builder.HasKey(o => o.Id);
        builder.HasIndex(o => o.Code).IsUnique();
        builder.Property(o => o.Code).HasMaxLength(50).IsRequired();
        builder.Property(o => o.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(o => o.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(o => o.NameEn).HasMaxLength(200);
        builder.Property(o => o.Icon).HasMaxLength(100);
    }
}
