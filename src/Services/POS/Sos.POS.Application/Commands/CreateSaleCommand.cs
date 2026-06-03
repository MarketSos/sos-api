using MediatR;
using Sos.POS.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.POS.Application.Commands;

public record CreateSaleCommand(Guid StoreId, Guid CashierId, Guid? CustomerId) : IRequest<Result<Guid>>;
public record AddSaleItemCommand(Guid SaleId, Guid ProductId, string ProductName, int Quantity, decimal UnitPrice) : IRequest<Result>;
public record CompleteSaleCommand(Guid SaleId, PaymentMethod Method, decimal PaidAmount) : IRequest<Result<CompleteSaleResponse>>;
public record CompleteSaleResponse(Guid SaleId, string ReceiptNumber, decimal TotalAmount, decimal Change