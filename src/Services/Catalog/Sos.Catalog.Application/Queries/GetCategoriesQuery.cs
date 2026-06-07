using MediatR;
using Sos.Catalog.Application.Interfaces;
using Sos.Catalog.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Catalog.Application.Queries;

public record CategoryDto(Guid Id, string NameUz, string NameRu, string? NameEn, Guid? ParentId);

public record GetCategoriesQuery : IRequest<Result<IEnumerable<CategoryDto>>>;

public class GetCategoriesHandler(ICategoryRepository repo)
    : IRequestHandler<GetCategoriesQuery, Result<IEnumerable<CategoryDto>>>
{
    public async Task<Result<IEnumerable<CategoryDto>>> Handle(GetCategoriesQuery _, CancellationToken ct)
    {
        var categories = await repo.GetAllAsync(ct);
        var dtos = categories.Select(c => new CategoryDto(c.Id, c.NameUz, c.NameRu, c.NameEn, c.ParentId));
        return Result.Success(dtos);
    }
}
