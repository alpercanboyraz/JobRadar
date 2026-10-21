using JobRadar.Domain.Jobs;

namespace JobRadar.Application.Abstractions;

/// <summary>İlan metninden yapısal bilgi çıkarır (LLM ile).</summary>
public interface IJobAnalyzer
{
    Task<JobAnalysis> AnalyzeAsync(JobPosting job, CancellationToken ct);
}
