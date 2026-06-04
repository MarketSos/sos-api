using Sos.Shared.Kernel.Domain;

namespace Sos.Loyalty.Domain.Entities;

/// <summary>
/// Mijozning loyallik hisobi — bonus ballar.
/// </summary>
public class LoyaltyAccount : AggregateRoot<Guid>
{
    public Guid    CustomerId  { get; private set; }
    public decimal Balance     { get; private set; }  // joriy bonus ballar
    public decimal TotalEarned { get; private set; }  // umrboqiy yig'ilgan
    public decimal TotalSpent  { get; private set; }  // umrboqiy sarflangan

    private readonly List<LoyaltyTransaction> _transactions = [];
    public IReadOnlyList<LoyaltyTransaction> Transactions => _transactions.AsReadOnly();

    private LoyaltyAccount() { }

    public static LoyaltyAccount Create(Guid customerId) => new()
    {
        Id         = Guid.NewGuid(),
        CustomerId = customerId,
        Balance    = 0,
        TotalEarned = 0,
        TotalSpent  = 0
    };

    public void Earn(decimal points, string description, Guid? saleId = null)
    {
        if (points <= 0) throw new ArgumentException("Earn points must be positive.");
        Balance     += points;
        TotalEarned += points;
        _transactions.Add(LoyaltyTransaction.Create(Id, points, LoyaltyTransactionType.Earn, description, saleId));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    public void Spend(decimal points, string description, Guid? saleId = null)
    {
        if (points <= 0) throw new ArgumentException("Spend points must be positive.");
        if (points > Balance) throw new InvalidOperationException("Yetarli bonus ball yo'q.");
        Balance    -= points;
        TotalSpent += points;
        _transactions.Add(LoyaltyTransaction.Create(Id, -points, LoyaltyTransactionType.Spend, description, saleId));
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public class LoyaltyTransaction : Entity<Guid>
{
    public Guid     AccountId   { get; private set; }
    public decimal  Points      { get; private set; }  // + earn, - spend
    public LoyaltyTransactionType Type { get; private set; }
    public string   Description { get; private set; } = default!;
    public Guid?    SaleId      { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private LoyaltyTransaction() { }

    public static LoyaltyTransaction Create(
        Guid accountId, decimal points, LoyaltyTransactionType type,
        string description, Guid? saleId) => new()
    {
        Id          = Guid.NewGuid(),
        AccountId   = accountId,
        Points      = points,
        Type        = type,
        Description = description,
        SaleId      = saleId,
        CreatedAt   = DateTimeOffset.UtcNow
    };
}

public enum LoyaltyTransactionType { Earn = 1, Spend = 2 }
