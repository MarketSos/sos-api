using MediatR;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

public record CreateProductCommand(
    string Name, string Barcode, Guid CategoryId,
    decimal BasePrice, string? SKU, string? Unit
) : IRequest<Result<Guid>>;

public class CreateProductHandler(
    Interfaces.IProductRepository repo) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByBarcodeAsync(cmd.Barcode, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"Barcode '{cmd.Barcode}' already exists.");

        var product = Domain.Entities.Product.Create(
            Guid.NewGuid(), cmd.Name, cmd.Barcode,
            cmd.CategoryId, cmd.BasePrice, cmd.SKU, cmd.Unit);

        await repo.AddAsync(product, ct);
        return Result.Success(product.Id);
    }
}
