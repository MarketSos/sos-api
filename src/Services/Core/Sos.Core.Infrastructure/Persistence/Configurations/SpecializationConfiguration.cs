using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class SpecializationConfiguration : IEntityTypeConfiguration<Specialization>
{
    public void Configure(EntityTypeBuilder<Specialization> builder)
    {
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Code).HasMaxLength(50).IsRequired();
        builder.HasIndex(s => new { s.Code, s.OrganizationId }).IsUnique();

        builder.Property(s => s.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(s => s.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(s => s.NameEn).HasMaxLength(200);
        builder.Property(s => s.NameUzCyrl).HasMaxLength(200);
        builder.Property(s => s.NameKk).HasMaxLength(200);
    }
}
