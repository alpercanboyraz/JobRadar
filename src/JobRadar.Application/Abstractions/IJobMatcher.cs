using JobRadar.Domain.Jobs;
using JobRadar.Domain.Matching;

namespace JobRadar.Application.Abstractions;

/// <summary>İlanı CV ile karşılaştırıp skor üretir (LLM ile).</summary>
public interface IJobMatcher
{
    Task<JobMatch> MatchAsync(JobPosting job, JobAnalysis analysis, CandidateProfile profile, CancellationToken ct);
}
