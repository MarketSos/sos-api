using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Exceptions;

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
        Id          = Guid.NewGuid(),
        CustomerId  = customerId,
        Balance     = 0,
        TotalEarned = 0,
        TotalSpent  = 0
    };

    /// <summary>
    /// Bonus ball yig'ish. points > 0 bo'lishi shart — domain invariant.
    /// </summary>
    public void Earn(decimal points, string description, Guid? saleId = null)
    {
        // Domain invariant: manfiy yoki nol ball yig'ish HECH QACHON bo'lmasligi kerak
        if (points <= 0)
            throw new DomainException($"Yig'iladigan ball musbat bo'lishi kerak. Berilgan: {points}");

        Balance     += points;
        TotalEarned += points;
        _transactions.Add(LoyaltyTransaction.Create(Id, points, LoyaltyTransactionType.Earn, description, saleId));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>
    /// Bonus ball sarflash. Balans yetarli bo'lishi shart — domain invariant.
    /// </summary>
    public void Spend(decimal points, string description, Guid? saleId = null)
    {
        // Domain invariant: manfiy sarflash bo'lmaydi
        if (points <= 0)
            throw new DomainException($"Sarflanadigan ball musbat bo'lishi kerak. Berilgan: {points}");

        // Domain invariant: balans hech qachon manfiy bo'la olmaydi
        if (points > Balance)
            throw new DomainException($"Balans yetarli emas. Mavjud: {Balance}, Kerak: {points}");

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
