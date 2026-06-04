using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Sos.Pricing.Domain.Entities;

namespace Sos.Pricing.Infrastructure.Persistence.Configurations;

public class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
{
    public void Configure(EntityTypeBuilder<PriceRule> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.ProductId).IsRequired();
        builder.Property(x => x.FixedPrice).HasPrecision(18, 4);
        builder.Property(x => x.DiscountPct).HasPrecision(5, 2);
        builder.Property(x => x.StartsAt).IsRequired();
        builder.HasIndex(x => new { x.ProductId, x.StoreId, x.IsActive });
    }
}
