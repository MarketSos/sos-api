using MediatR;
using Microsoft.EntityFrameworkCore;
using Sos.Identity.Domain.Entities;
using Sos.Shared.Infrastructure.Persistence;
using Sos.Shared.Infrastructure.Services;

namespace Sos.Identity.Infrastructure.Persistence;

public class IdentityDbContext(
    DbContextOptions<IdentityDbContext> options,
    IMediator mediator,
    ICurrentUserService currentUser)
    : BaseDbContext(options, mediator, currentUser)
{
    public DbSet<User>         Users         => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(IdentityDbContext).Assembly);
    }
}
