using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Exceptions;

namespace Sos.Commerce.Domain.Entities;

/// <summary>
/// Mijoz loyallik hisobi.
/// Лицевой счёт лояльности покупателя.
/// </summary>
public class LoyaltyAccount : AggregateRoot<Guid>, IHasOrganization
{
    public Guid    CustomerId     { get; private set; }
    public Guid    OrganizationId { get; set; }
    public decimal Balance        { get; private set; }
    public decimal TotalEarned { get; private set; }
    public decimal TotalSpent  { get; private set; }

    private readonly List<LoyaltyTransaction> _transactions = [];
    public IReadOnlyList<LoyaltyTransaction> Transactions => _transactions.AsReadOnly();

    private LoyaltyAccount() { }

    public static LoyaltyAccount Create(Guid customerId)
        => new() { Id = Guid.NewGuid(), CustomerId = customerId };

    /// <summary>Ball yig'ish. / Начислить баллы.</summary>
    public void Earn(decimal points, string description, Guid? saleId = null)
    {
        if (points <= 0) throw new DomainException($"Ball musbat bo'lishi kerak. Berilgan: {points}");
        Balance     += points;
        TotalEarned += points;
        _transactions.Add(LoyaltyTransaction.Create(Id, points, LoyaltyTransactionType.Earn, description, saleId));
        UpdatedAt = DateTimeOffset.UtcNow;
    }

    /// <summary>Ball sarflash. / Списать баллы.</summary>
    public void Spend(decimal points, string description, Guid? saleId = null)
    {
        if (points <= 0)    throw new DomainException($"Ball musbat bo'lishi kerak. Berilgan: {points}");
        if (points > Balance) throw new DomainException($"Balans yetarli emas. Mavjud: {Balance}, Kerak: {points}");
        Balance    -= points;
        TotalSpent += points;
        _transactions.Add(LoyaltyTransaction.Create(Id, -points, LoyaltyTransactionType.Spend, description, saleId));
        UpdatedAt = DateTimeOffset.UtcNow;
    }
}

public class LoyaltyTransaction : Entity<Guid>
{
    public Guid                  AccountId   { get; private set; }
    public decimal               Points      { get; private set; }
    public LoyaltyTransactionType Type       { get; private set; }
    public string                Description { get; private set; } = default!;
    public Guid?                 SaleId      { get; private set; }
    public DateTimeOffset        CreatedAt   { get; private set; }

    private LoyaltyTransaction() { }

    public static LoyaltyTransaction Create(
        Guid accountId, decimal points, LoyaltyTransactionType type,
        string description, Guid? saleId)
        => new()
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
