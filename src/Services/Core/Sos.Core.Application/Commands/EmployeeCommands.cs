using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Entities.Identity;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── CreateEmployee ────────────────────────────────────────────────────────────
public record CreateEmployeeCommand(
    Guid      UserId,
    string    FirstName,
    string    LastName,
    string?   MiddleName       = null,
    DateOnly? BirthDate        = null,
    string?   Phone            = null,
    Gender?   Gender           = null,
    Guid?     SpecializationId  = null,
    Guid?     EmployeeRankId    = null,
    DateOnly? HireDate         = null
) : IRequest<Result<Guid>>;

public class CreateEmployeeHandler(
    IEmployeeRepository repo,
    IUserRepository     userRepo,
    ICurrentContext     context)
    : IRequestHandler<CreateEmployeeCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeCommand cmd, CancellationToken ct)
    {
        var user = await userRepo.GetByIdAsync(cmd.UserId, ct);
        if (user is null) return Result.NotFound<Guid, User>(cmd.UserId);

        if (await repo.ExistsByUserIdAsync(cmd.UserId, ct))
            return Result.Conflict<Guid, User>(cmd.UserId);

        var employee = Employee.Create(
            cmd.UserId, cmd.FirstName, cmd.LastName,
            cmd.MiddleName, cmd.BirthDate, cmd.Phone, cmd.Gender,
            cmd.SpecializationId, cmd.EmployeeRankId, cmd.HireDate);

        if (context.OrganizationId.HasValue)
            employee.OrganizationId = context.OrganizationId.Value;

        await repo.AddAsync(employee, ct);
        return Result.Success(employee.Id);
    }
}

// ── UpdateEmployee ────────────────────────────────────────────────────────────
public record UpdateEmployeeCommand(
    Guid      Id,
    string    FirstName,
    string    LastName,
    string?   MiddleName      = null,
    DateOnly? BirthDate       = null,
    string?   Phone           = null,
    Gender?   Gender          = null,
    Guid?     SpecializationId = null,
    Guid?     EmployeeRankId   = null
) : IRequest<Result>;

public class UpdateEmployeeHandler(IEmployeeRepository repo)
    : IRequestHandler<UpdateEmployeeCommand, Result>
{
    public async Task<Result> Handle(UpdateEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await repo.GetByIdAsync(cmd.Id, ct);
        if (employee is null) return Result.NotFound<Employee>(cmd.Id);

        employee.Update(
            cmd.FirstName, cmd.LastName,
            cmd.MiddleName, cmd.BirthDate, cmd.Phone, cmd.Gender,
            cmd.SpecializationId, cmd.EmployeeRankId);

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── HireEmployee ──────────────────────────────────────────────────────────────
public record HireEmployeeCommand(Guid Id, DateOnly HireDate) : IRequest<Result>;

public class HireEmployeeHandler(IEmployeeRepository repo)
    : IRequestHandler<HireEmployeeCommand, Result>
{
    public async Task<Result> Handle(HireEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await repo.GetByIdAsync(cmd.Id, ct);
        if (employee is null) return Result.NotFound<Employee>(cmd.Id);

        employee.Hire(cmd.HireDate);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── FireEmployee ──────────────────────────────────────────────────────────────
public record FireEmployeeCommand(Guid Id, DateOnly FireDate) : IRequest<Result>;

public class FireEmployeeHandler(IEmployeeRepository repo)
    : IRequestHandler<FireEmployeeCommand, Result>
{
    public async Task<Result> Handle(FireEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await repo.GetByIdAsync(cmd.Id, ct);
        if (employee is null) return Result.NotFound<Employee>(cmd.Id);

        employee.Fire(cmd.FireDate);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteEmployee ────────────────────────────────────────────────────────────
public record DeleteEmployeeCommand(Guid Id) : IRequest<Result>;

public class DeleteEmployeeHandler(IEmployeeRepository repo, ICurrentContext context)
    : IRequestHandler<DeleteEmployeeCommand, Result>
{
    public async Task<Result> Handle(DeleteEmployeeCommand cmd, CancellationToken ct)
    {
        var employee = await repo.GetByIdAsync(cmd.Id, ct);
        if (employee is null) return Result.NotFound<Employee>(cmd.Id);

        employee.SoftDelete(context.UserId);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
