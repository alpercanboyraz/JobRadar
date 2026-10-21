using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace JobRadar.Infrastructure.Persistence;

/// <summary>Sadece `dotnet ef` komutları için. Uygulama çalışırken kullanılmaz.</summary>
internal sealed class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<JobRadarDbContext>
{
    public JobRadarDbContext CreateDbContext(string[] args)
    {
        var cs = Environment.GetEnvironmentVariable("JOBRADAR_DB")
                 ?? "Host=localhost;Port=5433;Database=jobradar;Username=jobradar;Password=jobradar_dev";

        var options = new DbContextOptionsBuilder<JobRadarDbContext>()
            .UseNpgsql(cs)
            .UseSnakeCaseNamingConvention()
            .Options;

        return new JobRadarDbContext(options);
    }
}
