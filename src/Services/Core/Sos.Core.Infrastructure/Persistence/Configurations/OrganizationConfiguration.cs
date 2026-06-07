using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class OrganizationConfiguration : IEntityTypeConfiguration<Organization>
{
    public void Configure(EntityTypeBuilder<Organization> builder)
    {
        builder.HasKey(o => o.Id);

        builder.HasIndex(o => o.Slug).IsUnique().HasFilter("\"Slug\" IS NOT NULL");
        builder.HasIndex(o => o.Code).IsUnique().HasFilter("\"Code\" IS NOT NULL");
        builder.HasIndex(o => o.OwnerUserId);
        builder.HasIndex(o => o.ParentId);

        builder.Property(o => o.NameUz).HasMaxLength(300).IsRequired();
        builder.Property(o => o.NameRu).HasMaxLength(300).IsRequired();
        builder.Property(o => o.NameEn).HasMaxLength(300);
        builder.Property(o => o.NameUzKiril).HasMaxLength(300);
        builder.Property(o => o.Slug).HasMaxLength(100);
        builder.Property(o => o.Code).HasMaxLength(50);
        builder.Property(o => o.Tin).HasMaxLength(20);
        builder.Property(o => o.Okonx).HasMaxLength(20);
        builder.Property(o => o.Oked).HasMaxLength(20);

        builder.HasOne(o => o.Parent)
               .WithMany(o => o.Childs)
               .HasForeignKey(o => o.ParentId)
               .OnDelete(DeleteBehavior.Restrict)
               .IsRequired(false);

        builder.HasOne(o => o.Address)
               .WithOne()
               .HasForeignKey<Organization>(o => o.AddressId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasOne<Sos.Core.Domain.Entities.OrgType>()
               .WithMany()
               .HasForeignKey(o => o.OrgTypeId)
               .OnDelete(DeleteBehavior.SetNull)
               .IsRequired(false);

        builder.HasMany(o => o.Members)
               .WithOne(m => m.Organization)
               .HasForeignKey(m => m.OrganizationId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
