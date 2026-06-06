using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record EmployeeDto(
    Guid      Id,
    Guid      UserId,
    string    Email,
    string    FirstName,
    string    LastName,
    string?   MiddleName,
    string    FullName,
    DateOnly? BirthDate,
    string?   Phone,
    string?   Gender,
    Guid?     SpecializationId,
    string?   SpecializationName,
    Guid?     EmployeeRankId,
    string?   EmployeeRankName,
    DateOnly? HireDate,
    DateOnly? FireDate,
    bool      IsActive,
    Guid      OrganizationId
);

public record EmployeeSummaryDto(
    Guid    Id,
    Guid    UserId,
    string  FullName,
    string? Phone,
    string? SpecializationName,
    string? EmployeeRankName,
    bool    IsActive
);

file static class Mapper
{
    internal static EmployeeDto ToDto(Employee e, string lang = "uz") => new(
        e.Id, e.UserId,
        e.User?.Email ?? "",
        e.FirstName, e.LastName, e.MiddleName, e.FullName,
        e.BirthDate, e.Phone, e.Gender?.ToString(),
        e.SpecializationId, e.Specialization?.GetName(lang),
        e.EmployeeRankId,   e.EmployeeRank?.GetName(lang),
        e.HireDate, e.FireDate, e.IsActive, e.OrganizationId);

    internal static EmployeeSummaryDto ToSummary(Employee e, string lang = "uz") => new(
        e.Id, e.UserId, e.FullName, e.Phone,
        e.Specialization?.GetName(lang),
        e.EmployeeRank?.GetName(lang),
        e.IsActive);
}

// ── GetEmployeeById ───────────────────────────────────────────────────────────
public record GetEmployeeByIdQuery(Guid Id, string Lang = "uz") : IRequest<Result<EmployeeDto>>;

public class GetEmployeeByIdHandler(IEmployeeRepository repo)
    : IRequestHandler<GetEmployeeByIdQuery, Result<EmployeeDto>>
{
    public async Task<Result<EmployeeDto>> Handle(GetEmployeeByIdQuery q, CancellationToken ct)
    {
        var e = await repo.GetByIdAsync(q.Id, ct);
        if (e is null) return Result.NotFound<EmployeeDto, Employee>(q.Id);
        return Result.Success(Mapper.ToDto(e, q.Lang));
    }
}

// ── GetMyEmployee ─────────────────────────────────────────────────────────────
public record GetMyEmployeeQuery(string Lang = "uz") : IRequest<Result<EmployeeDto>>;

public class GetMyEmployeeHandler(IEmployeeRepository repo, ICurrentContext context)
    : IRequestHandler<GetMyEmployeeQuery, Result<EmployeeDto>>
{
    public async Task<Result<EmployeeDto>> Handle(GetMyEmployeeQuery q, CancellationToken ct)
    {
        if (context.UserId is null) return Result.Failure<EmployeeDto>("Autentifikatsiya talab qilinadi.");
        var e = await repo.GetByUserIdAsync(context.UserId.Value, ct);
        if (e is null) return Result.NotFound<EmployeeDto, Employee>(context.UserId);
        return Result.Success(Mapper.ToDto(e, q.Lang));
    }
}

// ── GetAllEmployees ───────────────────────────────────────────────────────────
public record GetAllEmployeesQuery(string Lang = "uz") : IRequest<Result<List<EmployeeSummaryDto>>>;

public class GetAllEmployeesHandler(IEmployeeRepository repo)
    : IRequestHandler<GetAllEmployeesQuery, Result<List<EmployeeSummaryDto>>>
{
    public async Task<Result<List<EmployeeSummaryDto>>> Handle(GetAllEmployeesQuery q, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return Result.Success(list.Select(e => Mapper.ToSummary(e, q.Lang)).ToList());
    }
}
