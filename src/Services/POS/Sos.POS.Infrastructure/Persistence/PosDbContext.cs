using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.POS.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.POS.Infrastructure.Persistence;

public class PosDbContext(
    DbContextOptions<PosDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<Sale> Sales => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("pos");

        builder.Entity<Sale>(e =>
        {
            e.HasKey(s => s.Id);
            e.Property(s => s.SubTotal).HasColumnType("decimal(18,2)");
            e.Property(s => s.DiscountAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.TaxAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.TotalAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.PaidAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.ChangeAmount).HasColumnType("decimal(18,2)");
            e.Property(s => s.Status).HasConversion<string>();
            e.Property(s => s.PaymentMethod).HasConversion<string>();
            e.Property(s => s.ReceiptNumber).HasMaxLength(100);

            // Items — owned collection
            e.HasMany(s => s.Items)
             .WithOne()
             .HasForeignKey(i => i.SaleId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SaleItem>(e =>
        {
            e.HasKey(i => i.Id);
            e.Property(i => i.ProductName).HasMaxLength(300).IsRequired();
            e.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");
            e.Property(i => i.DiscountAmount).HasColumnType("decimal(18,2)");
            // TotalPrice — computed, EF dan hisoblab olamiz
            e.Ignore(i => i.TotalPrice);
        });
    }
}
