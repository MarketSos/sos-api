using FluentValidation;
using MediatR;
using Sos.Shared.Kernel.Exceptions;
using ValidationException = Sos.Shared.Kernel.Exceptions.ValidationException;

namespace Sos.Shared.Infrastructure.Behaviors;

/// <summary>
/// MediatR pipeline behavior — har bir request handler-dan OLDIN ishga tushadi.
/// Agar request uchun FluentValidation validator ro'yxatdan o'tgan bo'lsa,
/// validatsiyadan o'tkazadi. Xato bo'lsa — ValidationException tashlaydi.
///
/// Bu infrastructure qatlami xulqidir, shuning uchun exception ishlatiladi
/// (handler-lar esa biznes xatolar uchun Result.Failure ishlatadilar).
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var failures = (await Task.WhenAll(
                validators.Select(v => v.ValidateAsync(context, cancellationToken))))
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        var errors = failures
            .Select(f => new ValidationError(
                Field:   ToCamelCase(f.PropertyName),
                Message: f.ErrorMessage))
            .ToList();

        throw new ValidationException(errors);
    }

    private static string ToCamelCase(string name)
    {
        if (string.IsNullOrEmpty(name)) return name;
        return char.ToLowerInvariant(name[0]) + name[1..];
    }
}
