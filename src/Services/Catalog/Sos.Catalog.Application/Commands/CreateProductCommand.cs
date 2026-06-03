using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Domain;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Commands;

// ── CreateProduct ─────────────────────────────────────────────────────────────
public record CreateProductCommand(
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameUzKiril,
    string  Barcode,
    Guid    CategoryId,
    Guid?   BrandId = null
) : LocalizableCommand(NameUz, NameRu, NameEn, NameUzKiril), IRequest<Result<Guid>>;

public class CreateProductHandler(IProductRepository repo)
    : IRequestHandler<CreateProductCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateProductCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByBarcodeAsync(cmd.Barcode, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"Barcode '{cmd.Barcode}' allaqachon mavjud.");

        var product = Product.Create(
            id:           Guid.NewGuid(),
            nameUz:       cmd.NameUz,
            nameRu:       cmd.NameRu,
            barcode:      cmd.Barcode,
            categoryId:   cmd.CategoryId,
            brandId:      cmd.BrandId,
            nameEn:       cmd.NameEn,
            nameUzKiril:  cmd.NameUzKiril);

        await repo.AddAsync(product, ct);
        return Result.Success(product.Id);
    }
}

// ── CreateSku (kirim) ─────────────────────────────────────────────────────────
public record CreateSkuCommand(
    Guid    ProductId,
    string  SerialNumber,
    Guid    MeasurementUnitId,
    decimal Amount,
    decimal CostPrice,
    decimal SalePrice,
    Guid?   SupplierId     = null,
    decimal? Weight        = null,
    DateTimeOffset? ExpirationDate = null
) : IRequest<Result<Guid>>;

public class CreateSkuHandler(ISkuRepository skuRepo, IProductRepository productRepo)
    : IRequestHandler<CreateSkuCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSkuCommand cmd, CancellationToken ct)
    {
        var product = await productRepo.GetByIdAsync(cmd.ProductId, ct);
        if (product is null)
            return Result.Failure<Guid>("Mahsulot topilmadi.");

        var existing = await skuRepo.GetBySerialNumberAsync(cmd.SerialNumber, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"'{cmd.SerialNumber}' seriya raqami allaqachon mavjud.");

        if (cmd.Amount <= 0)
            return Result.Failure<Guid>("Miqdor 0 dan katta bo'lishi kerak.");

        var sku = Sku.Create(
            productId:         cmd.ProductId,
            serialNumber:      cmd.SerialNumber,
            measurementUnitId: cmd.MeasurementUnitId,
            amount:            cmd.Amount,
            costPrice:         cmd.CostPrice,
            salePrice:         cmd.SalePrice,
            supplierId:        cmd.SupplierId,
            weight:            cmd.Weight,
            expirationDate:    cmd.ExpirationDate);

        await skuRepo.AddAsync(sku, ct);
        return Result.Success(sku.Id);
    }
}

// ── CreateMeasurementUnit ─────────────────────────────────────────────────────
public record CreateMeasurementUnitCommand(
    string Code,
    string NameUz,
    string NameRu,
    string? NameEn = null,
    bool IsWeightBased = false
) : IRequest<Result<Guid>>;

public class CreateMeasurementUnitHandler(IMeasurementUnitRepository repo)
    : IRequestHandler<CreateMeasurementUnitCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateMeasurementUnitCommand cmd, CancellationToken ct)
    {
        var existing = await repo.GetByCodeAsync(cmd.Code, ct);
        if (existing is not null)
            return Result.Failure<Guid>($"'{cmd.Code}' kodi allaqachon mavjud.");

        var unit = MeasurementUnit.Create(Guid.NewGuid(), cmd.Code, cmd.NameUz, cmd.NameRu, cmd.NameEn, cmd.IsWeightBased);
        await repo.AddAsync(unit, ct);
        return Result.Success(unit.Id);
    }
}
