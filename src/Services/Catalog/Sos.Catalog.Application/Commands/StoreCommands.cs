using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── CreateStore ───────────────────────────────────────────────────────────────
public record CreateStoreCommand(
    Guid    OrganizationId,
    string  Code,
    string  Name,
    string? Address = null,
    string? Phone   = null
) : IRequest<Result<Guid>>;

public class CreateStoreHandler(IStoreRepository repo)
    : IRequestHandler<CreateStoreCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateStoreCommand cmd, CancellationToken ct)
    {
        var code = cmd.Code.Trim().ToUpperInvariant();
        if (await repo.CodeExistsAsync(cmd.OrganizationId, code, ct))
            return Result.Conflict<Guid, Store>(code);

        var store = Store.Create(cmd.OrganizationId, code, cmd.Name, cmd.Address, cmd.Phone);
        await repo.AddAsync(store, ct);
        return Result.Success(store.Id);
    }
}

// ── UpdateStore ───────────────────────────────────────────────────────────────
public record UpdateStoreCommand(
    Guid    Id,
    string  Code,
    string  Name,
    string? Address = null,
    string? Phone   = null
) : IRequest<Result>;

public class UpdateStoreHandler(IStoreRepository repo)
    : IRequestHandler<UpdateStoreCommand, Result>
{
    public async Task<Result> Handle(UpdateStoreCommand cmd, CancellationToken ct)
    {
        var store = await repo.GetByIdAsync(cmd.Id, ct);
        if (store is null) return Result.NotFound<Store>(cmd.Id);

        var code = cmd.Code.Trim().ToUpperInvariant();
        if (store.Code != code && await repo.CodeExistsAsync(store.OrganizationId, code, ct))
            return Result.Conflict<Store>(code);

        store.Update(code, cmd.Name, cmd.Address, cmd.Phone);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── DeleteStore ───────────────────────────────────────────────────────────────
public record DeleteStoreCommand(Guid Id) : IRequest<Result>;

public class DeleteStoreHandler(IStoreRepository repo)
    : IRequestHandler<DeleteStoreCommand, Result>
{
    public async Task<Result> Handle(DeleteStoreCommand cmd, CancellationToken ct)
    {
        var store = await repo.GetByIdAsync(cmd.Id, ct);
        if (store is null) return Result.NotFound<Store>(cmd.Id);

        store.SoftDelete();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

// ── ActivateStore / DeactivateStore ───────────────────────────────────────────
public record ActivateStoreCommand(Guid Id)   : IRequest<Result>;
public record DeactivateStoreCommand(Guid Id) : IRequest<Result>;

public class ActivateStoreHandler(IStoreRepository repo)
    : IRequestHandler<ActivateStoreCommand, Result>
{
    public async Task<Result> Handle(ActivateStoreCommand cmd, CancellationToken ct)
    {
        var store = await repo.GetByIdAsync(cmd.Id, ct);
        if (store is null) return Result.NotFound<Store>(cmd.Id);
        store.Activate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}

public class DeactivateStoreHandler(IStoreRepository repo)
    : IRequestHandler<DeactivateStoreCommand, Result>
{
    public async Task<Result> Handle(DeactivateStoreCommand cmd, CancellationToken ct)
    {
        var store = await repo.GetByIdAsync(cmd.Id, ct);
        if (store is null) return Result.NotFound<Store>(cmd.Id);
        store.Deactivate();
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
