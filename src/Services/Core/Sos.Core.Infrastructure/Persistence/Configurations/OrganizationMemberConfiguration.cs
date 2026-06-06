using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class OrganizationMemberConfiguration : IEntityTypeConfiguration<OrganizationMember>
{
    public void Configure(EntityTypeBuilder<OrganizationMember> builder)
    {
        builder.HasKey(m => m.Id);
        builder.HasIndex(m => new { m.OrganizationId, m.UserId }).IsUnique();
        builder.HasIndex(m => m.UserId);
        builder.Property(m => m.Role).IsRequired();
        builder.Property(m => m.JoinedAt).IsRequired();
    }
}
