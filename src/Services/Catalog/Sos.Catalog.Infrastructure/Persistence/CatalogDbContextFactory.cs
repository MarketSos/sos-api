using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sos.Catalog.Infrastructure.Persistence;

public class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<CatalogDbContext>()
            .UseNpgsql("Host=localhost;Database=SosCatalogDb;Username=postgres;Password=postgres")
            .Options;

        return new CatalogDbContext(opts, null!, null!);
    }
}
