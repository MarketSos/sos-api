using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record SpecializationDto(
    Guid    Id,
    string  Code,
    string  NameUz,
    string  NameRu,
    string? NameEn,
    string? NameUzKiril
);

file static class SpecializationMapper
{
    internal static SpecializationDto ToDto(Specialization s) => new(
        s.Id, s.Code, s.NameUz, s.NameRu, s.NameEn, s.NameUzKiril);
}

// ── GetSpecializationById ─────────────────────────────────────────────────────
public record GetSpecializationByIdQuery(Guid Id) : IRequest<Result<SpecializationDto>>;

public class GetSpecializationByIdHandler(ISpecializationRepository repo)
    : IRequestHandler<GetSpecializationByIdQuery, Result<SpecializationDto>>
{
    public async Task<Result<SpecializationDto>> Handle(GetSpecializationByIdQuery q, CancellationToken ct)
    {
        var s = await repo.GetByIdAsync(q.Id, ct);
        if (s is null) return Result.NotFound<SpecializationDto, Specialization>(q.Id);
        return Result.Success(SpecializationMapper.ToDto(s));
    }
}

// ── GetAllSpecializations ─────────────────────────────────────────────────────
public record GetAllSpecializationsQuery : IRequest<Result<List<SpecializationDto>>>;

public class GetAllSpecializationsHandler(ISpecializationRepository repo)
    : IRequestHandler<GetAllSpecializationsQuery, Result<List<SpecializationDto>>>
{
    public async Task<Result<List<SpecializationDto>>> Handle(GetAllSpecializationsQuery _, CancellationToken ct)
    {
        var list = await repo.GetAllAsync(ct);
        return Result.Success(list.Select(SpecializationMapper.ToDto).ToList());
    }
}
