using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Loyalty.Domain.Entities;

namespace Sos.Loyalty.Infrastructure.Persistence.Configurations;

public class LoyaltyAccountConfiguration : IEntityTypeConfiguration<LoyaltyAccount>
{
    public void Configure(EntityTypeBuilder<LoyaltyAccount> builder)
    {
        builder.HasKey(x => x.Id);
        builder.HasIndex(x => x.CustomerId).IsUnique();
        builder.Property(x => x.Balance).HasPrecision(18, 4);
        builder.Property(x => x.TotalEarned).HasPrecision(18, 4);
        builder.Property(x => x.TotalSpent).HasPrecision(18, 4);

        builder.HasMany(x => x.Transactions)
               .WithOne()
               .HasForeignKey(t => t.AccountId)
               .OnDelete(DeleteBehavior.Cascade);
    }
}

public class LoyaltyTransactionConfiguration : IEntityTypeConfiguration<LoyaltyTransaction>
{
    public void Configure(EntityTypeBuilder<LoyaltyTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Points).HasPrecision(18, 4);
        builder.Property(x => x.Description).HasMaxLength(300);
        builder.HasIndex(x => x.AccountId);
    }
}
