using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record GetProductByBarcodeQuery(string Barcode) : IRequest<Result<Product>>;

public class GetProductByBarcodeHandler(IProductRepository repo)
    : IRequestHandler<GetProductByBarcodeQuery, Result<Product>>
{
    public async Task<Result<Product>> Handle(GetProductByBarcodeQuery q, CancellationToken ct)
    {
        var product = await repo.GetByBarcodeAsync(q.Barcode, ct);
        return product is null
            ? Result.Failure<Product>($"Product with barcode '{q.Barcode}' not found.")
            : Result.Success(p