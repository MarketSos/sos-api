using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Core.Domain.Entities;

namespace Sos.Core.Infrastructure.Persistence.Configurations;

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(t => t.Id);
        builder.HasIndex(t => t.Token).IsUnique();
        builder.Property(t => t.Token).HasMaxLength(500).IsRequired();

        builder.HasOne<User>()
               .WithMany()
               .HasForeignKey(t => t.UserId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}
