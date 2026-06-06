using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FirstName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.LastName).HasMaxLength(100).IsRequired();
        builder.Property(e => e.MiddleName).HasMaxLength(100);
        builder.Property(e => e.Phone).HasMaxLength(30);
        builder.Property(e => e.Gender).HasConversion<string>().HasMaxLength(10);

        // 1-to-1: Employee ↔ User
        builder.HasOne(e => e.User)
            .WithOne()
            .HasForeignKey<Employee>(e => e.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(e => e.UserId).IsUnique();

        builder.HasOne(e => e.Specialization)
            .WithMany()
            .HasForeignKey(e => e.SpecializationId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(e => e.EmployeeRank)
            .WithMany()
            .HasForeignKey(e => e.EmployeeRankId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
