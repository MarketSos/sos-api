using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── CreateBrand ───────────────────────────────────────────────────────────────
public record CreateBrandCommand(
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result<Guid>>;

public class CreateBrandHandler(IBrandRepository repo)
    : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateBrandCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.Trim().ToUpperInvariant();
        if (await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Guid, Brand>(code);

        var brand = Brand.Create(Guid.NewGuid(), code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);

        await repo.AddAsync(brand, ct);
        return Result.Success(brand.Id);
    }
}

// ── UpdateBrand ───────────────────────────────────────────────────────────────
public record UpdateBrandCommand(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn      = null,
    string? NameUzKiril = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result>;

public class UpdateBrandHandler(IBrandRepository repo)
    : IRequestHandler<UpdateBrandCommand, Result>
{
    public async Task<Result> Handle(UpdateBrandCommand cmd, CancellationToken ct)
    {
        var brand = await repo.GetByIdAsync(cmd.Id, ct);
        if (brand is null) return Result.NotFound<Brand>(cmd.Id);

        var code = cmd.Code.Trim().ToUpperInvariant();
        if (brand.Code != code && await repo.CodeExistsAsync(code, ct))
            return Result.Conflict<Brand>(code);

        brand.Update(code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.NameUzKiril);

        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteBrand ───────────────────────────────────────────────────────────────
public record DeleteBrandCommand(Guid Id) : IRequest<Result>;

public class DeleteBrandHandler(IBrandRepository repo)
    : IRequestHandler<DeleteBrandCommand, Result>
{
    public async Task<Result> Handle(DeleteBrandCommand cmd, CancellationToken ct)
    {
        var brand = await repo.GetByIdAsync(cmd.Id, ct);
        if (brand is null) return Result.NotFound<Brand>(cmd.Id);

        brand.SoftDelete();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
