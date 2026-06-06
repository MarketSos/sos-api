using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Commands;

// ── CreateSpecialization ──────────────────────────────────────────────────────
public record CreateSpecializationCommand(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result<Guid>>;

public class CreateSpecializationHandler(ISpecializationRepository repo)
    : IRequestHandler<CreateSpecializationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSpecializationCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.Trim().ToUpperInvariant();
        if (await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Guid, Specialization>(code);

        var spec = Specialization.Create(Guid.NewGuid(), code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);
        await repo.AddAsync(spec, ct);
        return Result.Success(spec.Id);
    }
}

// ── UpdateSpecialization ──────────────────────────────────────────────────────
public record UpdateSpecializationCommand(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result>;

public class UpdateSpecializationHandler(ISpecializationRepository repo)
    : IRequestHandler<UpdateSpecializationCommand, Result>
{
    public async Task<Result> Handle(UpdateSpecializationCommand cmd, CancellationToken ct)
    {
        var spec = await repo.GetByIdAsync(cmd.Id, ct);
        if (spec is null) return Result.NotFound<Specialization>(cmd.Id);

        var code = cmd.Code.Trim().ToUpperInvariant();
        if (spec.Code != code && await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Specialization>(code);

        spec.Update(code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteSpecialization ──────────────────────────────────────────────────────
public record DeleteSpecializationCommand(Guid Id) : IRequest<Result>;

public class DeleteSpecializationHandler(ISpecializationRepository repo)
    : IRequestHandler<DeleteSpecializationCommand, Result>
{
    public async Task<Result> Handle(DeleteSpecializationCommand cmd, CancellationToken ct)
    {
        var spec = await repo.GetByIdAsync(cmd.Id, ct);
        if (spec is null) return Result.NotFound<Specialization>(cmd.Id);

        spec.SoftDelete();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
