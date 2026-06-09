using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── CreateManufacturer ────────────────────────────────────────────────────────
public record CreateManufacturerCommand(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzCyrl = null,
    string? NameKk      = null,
    string? AddressLine = null,
    string? Phone       = null
) : LocalizableCommand(NameUz, NameUzCyrl, NameRu, NameEn, NameKk), IRequest<Result<Guid>>;

public class CreateManufacturerHandler(IManufacturerRepository repo)
    : IRequestHandler<CreateManufacturerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateManufacturerCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.Trim().ToUpperInvariant();
        if (await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Guid, Manufacturer>(code);

        var manufacturer = Manufacturer.Create(
            Guid.NewGuid(), code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzCyrl, cmd.NameKk, cmd.AddressLine, cmd.Phone);

        await repo.AddAsync(manufacturer, ct);
        return Result.Success(manufacturer.Id);
    }
}

// ── UpdateManufacturer ────────────────────────────────────────────────────────
public record UpdateManufacturerCommand(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzCyrl = null,
    string? NameKk      = null,
    string? AddressLine = null,
    string? Phone       = null
) : LocalizableCommand(NameUz, NameUzCyrl, NameRu, NameEn, NameKk), IRequest<Result>;

public class UpdateManufacturerHandler(IManufacturerRepository repo)
    : IRequestHandler<UpdateManufacturerCommand, Result>
{
    public async Task<Result> Handle(UpdateManufacturerCommand cmd, CancellationToken ct)
    {
        var manufacturer = await repo.GetByIdAsync(cmd.Id, ct);
        if (manufacturer is null) return Result.NotFound<Manufacturer>(cmd.Id);

        var code = cmd.Code.Trim().ToUpperInvariant();
        if (manufacturer.Code != code && await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Manufacturer>(code);

        manufacturer.Update(code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzCyrl, cmd.NameKk);
        manufacturer.SetAddress(cmd.AddressLine);
        manufacturer.SetPhone(cmd.Phone);

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteManufacturer ────────────────────────────────────────────────────────
public record DeleteManufacturerCommand(Guid Id) : IRequest<Result>;

public class DeleteManufacturerHandler(IManufacturerRepository repo)
    : IRequestHandler<DeleteManufacturerCommand, Result>
{
    public async Task<Result> Handle(DeleteManufacturerCommand cmd, CancellationToken ct)
    {
        var manufacturer = await repo.GetByIdAsync(cmd.Id, ct);
        if (manufacturer is null) return Result.NotFound<Manufacturer>(cmd.Id);

        manufacturer.SoftDelete();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
