using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sos.Core.Infrastructure.Persistence;

public class CoreDbContextFactory : IDesignTimeDbContextFactory<CoreDbContext>
{
    public CoreDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<CoreDbContext>()
            .UseNpgsql("Host=localhost;Database=SosCoreDb;Username=postgres;Password=postgres")
            .Options;

        return new CoreDbContext(opts, null!, null!);
    }
}
