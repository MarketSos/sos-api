using Microsoft.EntityFrameworkCore;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Commerce.Infrastructure.Persistence;

namespace Sos.Commerce.Infrastructure.Repositories;

public class CustomerRepository(CommerceDbContext db) : ICustomerRepository
{
    public Task<Customer?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => db.Customers.AsTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public Task<Customer?> GetByPhoneAsync(string phone, CancellationToken ct = default)
        => db.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phone, ct);

    public async Task<List<Customer>> SearchAsync(string query, int limit, CancellationToken ct = default)
    {
        var q = query.ToLower();
        return await db.Customers
            .Where(c => c.FirstName.ToLower().Contains(q)
                     || c.LastName.ToLower().Contains(q)
                     || (c.PhoneNumber != null && c.PhoneNumber.Contains(q)))
            .Take(limit)
            .ToListAsync(ct);
    }

    public async Task AddAsync(Customer customer, CancellationToken ct = default)
        => await db.Customers.AddAsync(customer, ct);

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
