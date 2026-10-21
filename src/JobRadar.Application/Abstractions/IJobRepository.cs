using JobRadar.Domain.Jobs;
using JobRadar.Domain.Matching;

namespace JobRadar.Application.Abstractions;

public interface IJobRepository
{
    Task<IReadOnlyList<JobPosting>> GetJobsAsync(JobSourceType source, string companyName, CancellationToken ct);

    void Add(JobPosting job);
    void AddAnalysis(JobAnalysis analysis);
    void AddMatch(JobMatch match);

    Task SaveChangesAsync(CancellationToken ct);
}
