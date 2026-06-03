using MediatR;
using Sos.POS.Application.Interfaces;
using Sos.POS.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.POS.Application.Commands;

// ── CreateSale ──────────────────────────────────────────────────────────────
public record CreateSaleCommand(Guid StoreId, Guid CashierId, Guid? CustomerId) : IRequest<Result<Guid>>;

public class CreateSaleHandler(ISaleRepository repo) : IRequestHandler<CreateSaleCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateSaleCommand cmd, CancellationToken ct)
    {
        var sale = Sale.Create(cmd.StoreId, cmd.CashierId, cmd.CustomerId);
        await repo.AddAsync(sale, ct);
        return Result.Success(sale.Id);
    }
}

// ── AddSaleItem ──────────────────────────────────────────────────────────────
public record AddSaleItemCommand(Guid SaleId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice)
    : IRequest<Result>;

public class AddSaleItemHandler(ISaleRepository repo) : IRequestHandler<AddSaleItemCommand, Result>
{
    public async Task<Result> Handle(AddSaleItemCommand cmd, CancellationToken ct)
    {
        var sale = await repo.GetByIdAsync(cmd.SaleId, ct);
        if (sale is null) return Result.Failure("Sale topilmadi.");
        if (sale.Status != SaleStatus.Draft) return Result.Failure("Faqat ochiq chekka tovar qo'shish mumkin.");

        sale.AddItem(cmd.ProductId, cmd.ProductName, cmd.Quantity, cmd.UnitPrice);
        await repo.UpdateAsync(sale, ct);
        return Result.Success();
    }
}

// ── CompleteSale ─────────────────────────────────────────────────────────────
public record CompleteSaleCommand(Guid SaleId, PaymentMethod Method, decimal PaidAmount)
    : IRequest<Result<CompleteSaleResponse>>;
public record CompleteSaleResponse(Guid SaleId, string ReceiptNumber, decimal TotalAmount, decimal ChangeAmount);

public class CompleteSaleHandler(ISaleRepository repo) : IRequestHandler<CompleteSaleCommand, Result<CompleteSaleResponse>>
{
    public async Task<Result<CompleteSaleResponse>> Handle(CompleteSaleCommand cmd, CancellationToken ct)
    {
        var sale = await repo.GetByIdAsync(cmd.SaleId, ct);
        if (sale is null) return Result.Failure<CompleteSaleResponse>("Sale topilmadi.");
        if (sale.Status != SaleStatus.Draft) return Result.Failure<CompleteSaleResponse>("Chek allaqachon yopilgan.");
        if (!sale.Items.Any()) return Result.Failure<CompleteSaleResponse>("Chekda tovarlar yo'q.");
        if (cmd.PaidAmount < sale.TotalAmount)
            return Result.Failure<CompleteSaleResponse>($"To'lov yetarli emas. Kerak: {sale.TotalAmount:N2}");

        sale.Complete(cmd.Method, cmd.PaidAmount);
        await repo.UpdateAsync(sale, ct);

        return Result.Success(new CompleteSaleResponse(
            sale.Id,
            sale.ReceiptNumber!,
            sale.TotalAmount,
            sale.ChangeAmount));
    }
}

// ── CancelSale ───────────────────────────────────────────────────────────────
public record CancelSaleCommand(Guid SaleId) : IRequest<Result>;

public class CancelSaleHandler(ISaleRepository repo) : IRequestHandler<CancelSaleCommand, Result>
{
    public async Task<Result> Handle(CancelSaleCommand cmd, CancellationToken ct)
    {
        var sale = await repo.GetByIdAsync(cmd.SaleId, ct);
        if (sale is null) return Result.Failure("Sale topilmadi.");
        if (sale.Status != SaleStatus.Draft) return Result.Failure("Faqat ochiq chekni bekor qilish mumkin.");

        sale.Cancel();
        await repo.UpdateAsync(sale, ct);
        return Result.Success();
    }
}
