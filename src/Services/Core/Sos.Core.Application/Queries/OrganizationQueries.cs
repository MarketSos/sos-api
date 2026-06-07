using MediatR;
using Sos.Core.Application.Interfaces;
using Sos.Core.Domain.Entities;
using Sos.Core.Domain.Enums;
using Sos.Shared.Infrastructure.Services;
using Sos.Shared.Kernel.Results;

namespace Sos.Core.Application.Queries;

// ── DTOs ──────────────────────────────────────────────────────────────────────
public record OrganizationDto(
    Guid               Id,
    string             NameUz,
    string             NameRu,
    string?            NameEn,
    string?            NameUzKiril,
    string?            Slug,
    string?            Code,
    string?            Tin,
    string?            Okonx,
    string?            Oked,
    OwnershipType?     Ownership,
    OrganizationLevel? Level,
    Guid?              OrgTypeId,
    bool               IsTest,
    bool               IsActive,
    Guid               OwnerUserId,
    Guid?              ParentId,
    Guid?              AddressId,
    IEnumerable<MemberDto> Members
);

public record MemberDto(Guid Id, Guid UserId, OrganizationRole Role, DateTimeOffset JoinedAt, bool IsActive);

public record OrganizationSummaryDto(
    Guid    Id,
    string  NameUz,
    string  NameRu,
    string? Slug,
    string? Code,
    bool    IsActive,
    Guid?   ParentId
);

// ── Mapper ────────────────────────────────────────────────────────────────────
file static class Mapper
{
    internal static OrganizationDto ToDto(Domain.Entities.Organization org)
        => new(
            org.Id, org.NameUz, org.NameRu, org.NameEn, org.NameUzKiril,
            org.Slug, org.Code, org.Tin, org.Okonx, org.Oked,
            org.Ownership, org.Level, org.OrgTypeId,
            org.IsTest, org.IsActive,
            org.OwnerUserId, org.ParentId, org.AddressId,
            org.Members.Select(m => new MemberDto(m.Id, m.UserId, m.Role, m.JoinedAt, m.IsActive)));

    internal static OrganizationSummaryDto ToSummary(Domain.Entities.Organization org)
        => new(org.Id, org.NameUz, org.NameRu, org.Slug, org.Code, org.IsActive, org.ParentId);
}

// ── GetMyOrganization ─────────────────────────────────────────────────────────
public record GetMyOrganizationQuery : IRequest<Result<OrganizationDto>>;

public class GetMyOrganizationHandler(IOrganizationRepository repo, ICurrentContext context)
    : IRequestHandler<GetMyOrganizationQuery, Result<OrganizationDto>>
{
    public async Task<Result<OrganizationDto>> Handle(GetMyOrganizationQuery _, CancellationToken ct)
    {
        if (context.OrganizationId is null)
            return Result.Failure<OrganizationDto>("Tashkilot aniqlanmadi.");

        var org = await repo.GetByIdAsync(context.OrganizationId.Value, ct);
        if (org is null) return Result.NotFound<OrganizationDto, Organization>(context.OrganizationId.Value);

        return Result.Success(Mapper.ToDto(org));
    }
}

// ── GetOrganizationById ───────────────────────────────────────────────────────
public record GetOrganizationByIdQuery(Guid Id) : IRequest<Result<OrganizationDto>>;

public class GetOrganizationByIdHandler(IOrganizationRepository repo)
    : IRequestHandler<GetOrganizationByIdQuery, Result<OrganizationDto>>
{
    public async Task<Result<OrganizationDto>> Handle(GetOrganizationByIdQuery q, CancellationToken ct)
    {
        var org = await repo.GetByIdAsync(q.Id, ct);
        if (org is null) return Result.NotFound<OrganizationDto, Organization>(q.Id);
        return Result.Success(Mapper.ToDto(org));
    }
}

// ── GetOrganizationByCode ─────────────────────────────────────────────────────
public record GetOrganizationByCodeQuery(string Code) : IRequest<Result<OrganizationDto>>;

public class GetOrganizationByCodeHandler(IOrganizationRepository repo)
    : IRequestHandler<GetOrganizationByCodeQuery, Result<OrganizationDto>>
{
    public async Task<Result<OrganizationDto>> Handle(GetOrganizationByCodeQuery q, CancellationToken ct)
    {
        var org = await repo.GetByCodeAsync(q.Code, ct);
        if (org is null) return Result.NotFound<OrganizationDto, Organization>(q.Code);
        return Result.Success(Mapper.ToDto(org));
    }
}

// ── GetAllOrganizations ───────────────────────────────────────────────────────
public record GetAllOrganizationsQuery : IRequest<Result<List<OrganizationSummaryDto>>>;

public class GetAllOrganizationsHandler(IOrganizationRepository repo)
    : IRequestHandler<GetAllOrganizationsQuery, Result<List<OrganizationSummaryDto>>>
{
    public async Task<Result<List<OrganizationSummaryDto>>> Handle(GetAllOrganizationsQuery _, CancellationToken ct)
    {
        var orgs = await repo.GetAllAsync(ct);
        return Result.Success(orgs.Select(Mapper.ToSummary).ToList());
    }
}

// ── GetOrganizationChilds ─────────────────────────────────────────────────────
public record GetOrganizationChildsQuery(Guid ParentId) : IRequest<Result<List<OrganizationSummaryDto>>>;

public class GetOrganizationChildsHandler(IOrganizationRepository repo)
    : IRequestHandler<GetOrganizationChildsQuery, Result<List<OrganizationSummaryDto>>>
{
    public async Task<Result<List<OrganizationSummaryDto>>> Handle(GetOrganizationChildsQuery q, CancellationToken ct)
    {
        var childs = await repo.GetChildsAsync(q.ParentId, ct);
        return Result.Success(childs.Select(Mapper.ToSummary).ToList());
    }
}
