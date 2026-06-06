using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Sos.Analytics.Infrastructure.Persistence;

public class AnalyticsDbContextFactory : IDesignTimeDbContextFactory<AnalyticsDbContext>
{
    public AnalyticsDbContext CreateDbContext(string[] args)
    {
        var opts = new DbContextOptionsBuilder<AnalyticsDbContext>()
            .UseNpgsql("Host=localhost;Database=SosAnalyticsDb;Username=postgres;Password=postgres")
            .Options;

        return new AnalyticsDbContext(opts, null!, null!);
    }
}
