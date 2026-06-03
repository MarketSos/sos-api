using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Infrastructure.Persistence.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NameUz).HasMaxLength(200).IsRequired();
        builder.Property(c => c.NameRu).HasMaxLength(200).IsRequired();
        builder.Property(c => c.NameEn).HasMaxLength(200);
        builder.Property(c => c.NameUzKiril).HasMaxLength(200);

        builder.HasMany(c => c.Children)
               .WithOne(c => c.Parent)
               .HasForeignKey(c => c.ParentId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}
