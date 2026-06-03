using MediatR;
using Sos.Shared.Kernel.Results;
using Sos.Shared.Kernel.Domain;
using Sos.Catalog.Domain.Entities;

namespace Sos.Catalog.Application.Commands;

public record CreateProductCommand(
    string NameUz, string NameRu, string? NameEn, string? NameUzKiril,
    string Barcode, Guid CategoryId,
    decimal BasePrice, string? SKU, string? Unit
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result<Guid>>;

public class CreateProductHandler(
    Interfaces.IProductRepository repo) : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByBarcodeAsync(cmd.Barcode, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"Barcode '{cmd.Barcode}' already exists.");

        var product = Product.Create(
            Guid.NewGuid(), 
            cmd.NameUz, 
            cmd.NameRu, 
            cmd.Barcode, 
            cmd.CategoryId, 
            cmd.BasePrice, 
            cmd.NameEn, 
            cmd.NameUzKiril,
            cmd.SKU, 
            cmd.Unit);

        await repo.AddAsync(product, ct);
        return Result.Success(product.Id);
    }
}
