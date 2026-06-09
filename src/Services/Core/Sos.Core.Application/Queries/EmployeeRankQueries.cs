using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record EmployeeRankDto(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameUzCyrl,
    string? NameKk
);

file static class EmployeeRankMapper
{
    internal static EmployeeRankDto ToDto(EmployeeRank r) => new(
        r.Id, r.Code, r.NameUz, r.NameRu, r.NameEn, r.NameUzCyrl, r.NameKk);
}

// ── GetEmployeeRankById ───────────────────────────────────────────────────────
public record GetEmployeeRankByIdQuery(Guid Id) : IRequest<Result<EmployeeRankDto>>;

public class GetEmployeeRankByIdHandler(IEmployeeRankRepository repo)
    : IRequestHandler<GetEmployeeRankByIdQuery, Result<EmployeeRankDto>>
{
    public async Task<Result<EmployeeRankDto>> Handle(GetEmployeeRankByIdQuery q, CancellationToken ct)
    {
        var r = await repo.GetByIdAsync(q.Id, ct);
        if (r is null) return Result.NotFound<EmployeeRankDto, EmployeeRank>(q.Id);
        return Result.Success(EmployeeRankMapper.ToDto(r));
    }
}

// ── GetAllEmployeeRanks ───────────────────────────────────────────────────────
public record GetAllEmployeeRanksQuery : IRequest<Result<List<EmployeeRankDto>>>;

public class GetAllEmployeeRanksHandler(IEmployeeRankRepository repo)
    : IRequestHandler<GetAllEmployeeRanksQuery, Result<List<EmployeeRankDto>>>
{
    public async Task<Result<List<EmployeeRankDto>>> Handle(GetAllEmployeeRanksQuery _, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return Result.Success(list.Select(EmployeeRankMapper.ToDto).ToList());
    }
}
