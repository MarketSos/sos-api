using Microsoft.EntityFrameworkCore;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Infrastructure.Persistence;

namespace Sos.Core.Infrastructure.Repositories;

public class EmployeeRepository(CoreDbContext db) : IEmployeeRepository
{
    private IQueryable<Employee> WithIncludes => db.Employees
        .Include(e => e.User)
        .Include(e => e.Specialization)
        .Include(e => e.EmployeeRank);

    public Task<Employee?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => WithIncludes.AsTracking().FirstOrDefaultAsync(e => e.Id == id, ct);

    public Task<Employee?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => WithIncludes.AsTracking().FirstOrDefaultAsync(e => e.UserId == userId, ct);

    public Task<List<Employee>> GetAllAsync(CancellationToken ct = default)
        => WithIncludes.OrderBy(e => e.LastName).ThenBy(e => e.FirstName).ToListAsync(ct);

    public Task<bool> ExistsByUserIdAsync(Guid userId, CancellationToken ct = default)
        => db.Employees.AnyAsync(e => e.UserId == userId, ct);

    public async Task AddAsync(Employee employee, CancellationToken ct = default)
    {
        await db.Employees.AddAsync(employee, ct);
        await db.SaveChangesAsync(ct);
    }

    public Task SaveChangesAsync(CancellationToken ct = default)
        => db.SaveChangesAsync(ct);
}
