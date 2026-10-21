using JobRadar.Application.Abstractions;
using JobRadar.Domain.Jobs;
using JobRadar.Domain.Matching;
using Microsoft.EntityFrameworkCore;

namespace JobRadar.Infrastructure.Persistence;

internal sealed class JobRepository(JobRadarDbContext db) : IJobRepository
{
    public async Task<IReadOnlyList<JobPosting>> GetJobsAsync(
        JobSourceType source, string companyName, CancellationToken ct) =>
        await db.JobPostings
            .Where(j => j.Source == source && j.CompanyName == companyName)
            .ToListAsync(ct);

    public void Add(JobPosting job) => db.JobPostings.Add(job);
    public void AddAnalysis(JobAnalysis analysis) => db.JobAnalyses.Add(analysis);
    public void AddMatch(JobMatch match) => db.JobMatches.Add(match);

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
