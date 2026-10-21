using JobRadar.Application.Abstractions;
using JobRadar.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JobRadar.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        var cs = config.GetConnectionString("JobRadar")
                 ?? throw new InvalidOperationException("ConnectionStrings:JobRadar is missing");

        services.AddDbContext<JobRadarDbContext>(o => o
            .UseNpgsql(cs)
            .UseSnakeCaseNamingConvention());

        services.AddScoped<IJobRepository, JobRepository>();

        return services;
    }
}
