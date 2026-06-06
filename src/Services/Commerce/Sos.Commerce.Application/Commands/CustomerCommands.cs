using MediatR;
using Sos.Commerce.Application.Interfaces;
using Sos.Commerce.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Commerce.Application.Commands;

// ── CreateCustomer ────────────────────────────────────────────────────────────
public record CreateCustomerCommand(
    string   FirstName,
    string   LastName,
    string?  PhoneNumber,
    string?  Email,
    DateOnly? BirthDate,
    Guid?    StoreId = null
) : IRequest<Result<Guid>>;

public class CreateCustomerHandler(ICustomerRepository repo)
    : IRequestHandler<CreateCustomerCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateCustomerCommand cmd, CancellationToken ct)
    {
        if (!string.IsNullOrWhiteSpace(cmd.PhoneNumber))
        {
            var existing = await repo.GetByPhoneAsync(cmd.PhoneNumber, ct);
            if (existing is not null)
                return Result.Failure<Guid>($"'{cmd.PhoneNumber}' raqamli mijoz allaqachon mavjud.");
        }

        var customer = Customer.Create(
            cmd.FirstName, cmd.LastName,
            cmd.PhoneNumber, cmd.Email,
            cmd.BirthDate, cmd.StoreId);

        await repo.AddAsync(customer, ct);
        await repo.SaveChangesAsync(ct);
        return Result.Success(customer.Id);
    }
}

// ── UpdateCustomer ────────────────────────────────────────────────────────────
public record UpdateCustomerCommand(
    Guid     CustomerId,
    string   FirstName,
    string   LastName,
    string?  PhoneNumber,
    string?  Email,
    DateOnly? BirthDate
) : IRequest<Result>;

public class UpdateCustomerHandler(ICustomerRepository repo)
    : IRequestHandler<UpdateCustomerCommand, Result>
{
    public async Task<Result> Handle(UpdateCustomerCommand cmd, CancellationToken ct)
    {
        var customer = await repo.GetByIdAsync(cmd.CustomerId, ct);
        if (customer is null) return Result.Failure("Mijoz topilmadi.");

        customer.Update(cmd.FirstName, cmd.LastName, cmd.PhoneNumber, cmd.Email, cmd.BirthDate);
        await repo.SaveChangesAsync(ct);
        return Result.Success();
    }
}
