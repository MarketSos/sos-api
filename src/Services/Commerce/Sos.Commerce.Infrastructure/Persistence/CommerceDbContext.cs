using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Commerce.Infrastructure.Persistence;

/// <summary>
/// POS + Loyalty + CRM uchun birlashtirilgan DbContext.
/// Объединённый DbContext для POS, Loyalty и CRM.
/// </summary>
public class CommerceDbContext(
    DbContextOptions<CommerceDbContext> options,
    IMediator       mediator,
    ICurrentContext currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    // POS
    public DbSet<Sale>     Sales     => Set<Sale>();
    public DbSet<SaleItem> SaleItems => Set<SaleItem>();

    // Loyalty
    public DbSet<LoyaltyAccount>     LoyaltyAccounts     => Set<LoyaltyAccount>();
    public DbSet<LoyaltyTransaction> LoyaltyTransactions => Set<LoyaltyTransaction>();

    // CRM
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(CommerceDbContext).Assembly);
    }
}
