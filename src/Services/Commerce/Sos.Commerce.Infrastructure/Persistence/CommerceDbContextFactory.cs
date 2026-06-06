using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sos.Commerce.Infrastructure.Persistence;

public class CommerceDbContextFactory : IDesignTimeDbContextFactory<CommerceDbContext>
{
    public CommerceDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<CommerceDbContext>()
            .UseNpgsql("Host=localhost;Database=SosCommerceDb;Username=postgres;Password=postgres")
            .Options;

        return new CommerceDbContext(opts, null!, null!);
    }
}
