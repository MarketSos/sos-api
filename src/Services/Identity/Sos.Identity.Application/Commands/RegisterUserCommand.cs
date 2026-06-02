using MediatR;
using Sos.Identity.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Identity.Application.Commands;

public record RegisterUserCommand(
    string Email, string Password,
    string FirstName, string LastName,
    UserRole Role, Guid? StoreId = null
) : IRequest<Result<Guid>>;
