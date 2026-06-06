using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class EmployeeRankConfiguration : IEntityTypeConfiguration<EmployeeRank>
{
    public void Configure(EntityTypeBuilder<EmployeeRank> builder)
    {
        builder.HasKey(r => r.Id);

        builder.Property(r => r.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(r => new { r.Code, r.OrganizationId }).IsUnique();

        builder.Property(r => r.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(r => r.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(r => r.NameEn).HasMaxLength(200);
        builder.Property(r => r.NameUzKiril).HasMaxLength(200);
    }
}
