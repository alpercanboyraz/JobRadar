using JobRadar.Domain.Jobs;
using JobRadar.Domain.Matching;
using Microsoft.EntityFrameworkCore;

namespace JobRadar.Infrastructure.Persistence;

public sealed class JobRadarDbContext(DbContextOptions<JobRadarDbContext> options) : DbContext(options)
{
    public DbSet<JobPosting> JobPostings => Set<JobPosting>();
    public DbSet<JobAnalysis> JobAnalyses => Set<JobAnalysis>();
    public DbSet<JobMatch> JobMatches => Set<JobMatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(JobRadarDbContext).Assembly);
    }
}
