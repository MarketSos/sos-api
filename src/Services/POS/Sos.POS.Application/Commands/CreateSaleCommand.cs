using MediatR;
using Sos.Shared.Kernel.Results;

namespace Sos.POS.Application.Commands;

public record CreateSaleCommand(Guid StoreId, Guid CashierId, Guid? CustomerId) : IRequest<Result<Guid>>;
public record AddSaleItemCommand(Guid SaleId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice) : IRequest<Result>;
public record CompleteSaleCommand(Guid SaleId, Domain.Entities.PaymentMethod Method, decimal PaidAmount) : IRequest<Result<CompleteSaleResponse>>;
public record CompleteSaleResponse(Guid SaleId, string ReceiptNumber, decimal TotalAmount, decimal ChangeAmount);
