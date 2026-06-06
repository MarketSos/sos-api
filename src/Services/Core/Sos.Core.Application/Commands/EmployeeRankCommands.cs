using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── CreateEmployeeRank ────────────────────────────────────────────────────────
public record CreateEmployeeRankCommand(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result<Guid>>;

public class CreateEmployeeRankHandler(IEmployeeRankRepository repo)
    : IRequestHandler<CreateEmployeeRankCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateEmployeeRankCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.Trim().ToUpperInvariant();
        if (await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Guid, EmployeeRank>(code);

        var rank = EmployeeRank.Create(Guid.NewGuid(), code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);
        await repo.AddAsync(rank, ct);
        return Result.Success(rank.Id);
    }
}

// ── UpdateEmployeeRank ────────────────────────────────────────────────────────
public record UpdateEmployeeRankCommand(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result>;

public class UpdateEmployeeRankHandler(IEmployeeRankRepository repo)
    : IRequestHandler<UpdateEmployeeRankCommand, Result>
{
    public async Task<Result> Handle(UpdateEmployeeRankCommand cmd, CancellationToken ct)
    {
        var rank = await repo.GetByIdAsync(cmd.Id, ct);
        if (rank is null) return Result.NotFound<EmployeeRank>(cmd.Id);

        var code = cmd.Code.Trim().ToUpperInvariant();
        if (rank.Code != code && await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<EmployeeRank>(code);

        rank.Update(code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteEmployeeRank ────────────────────────────────────────────────────────
public record DeleteEmployeeRankCommand(Guid Id) : IRequest<Result>;

public class DeleteEmployeeRankHandler(IEmployeeRankRepository repo)
    : IRequestHandler<DeleteEmployeeRankCommand, Result>
{
    public async Task<Result> Handle(DeleteEmployeeRankCommand cmd, CancellationToken ct)
    {
        var rank = await repo.GetByIdAsync(cmd.Id, ct);
        if (rank is null) return Result.NotFound<EmployeeRank>(cmd.Id);

        rank.SoftDelete();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
