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
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.HasDefaultSchema("identity");

        builder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.Property(u => u.Email).HasMaxLength(256).IsRequired();
            e.Property(u => u.FirstName).HasMaxLength(100).IsRequired();
            e.Property(u => u.LastName).HasMaxLength(100).IsRequired();
            e.Property(u => u.Role).HasConversion<string>();
        });

        builder.Entity<RefreshToken>(e =>
        {
            e.HasKey(t => t.Id);
            e.HasIndex(t => t.Token).IsUnique();
            e.Property(t => t.Token).HasMaxLength(500).IsRequired();
            e.HasOne<User>().WithMany().HasForeignKey(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
        });
    }
}
