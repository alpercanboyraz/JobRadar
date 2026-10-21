using JobRadar.Application.Abstractions;
using JobRadar.Domain.Jobs;
using Microsoft.Extensions.Logging;

namespace JobRadar.Application.Scanning;

public sealed class DailyScanService(
    IEnumerable<IJobSource> sources,
    IJobRepository repository,
    IJobAnalyzer analyzer,
    IJobMatcher matcher,
    ICandidateProfileProvider profileProvider,
    TimeProvider time,
    ILogger<DailyScanService> logger)
{
    private readonly Dictionary<JobSourceType, IJobSource> _sources = sources.ToDictionary(s => s.Type);

    public async Task<ScanReport> RunAsync(IReadOnlyList<TrackedCompany> companies, CancellationToken ct)
    {
        var report = new ScanReport();
        var profile = await profileProvider.GetAsync(ct);

        foreach (var company in companies)
        {
            // Bir şirket patlarsa diğerleri etkilenmesin
            try
            {
                var changed = await SyncCompanyAsync(company, report, ct);

                foreach (var job in changed)
                    await AnalyzeAndMatchAsync(job, profile, report, ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                report.FailedCompanies++;
                logger.LogError(ex, "Scan failed for {Company}", company.Name);
            }
        }

        logger.LogInformation("Daily scan finished: {Report}", report);
        return report;
    }

    /// <summary>Kaynakla DB'yi senkronize eder. LLM'e gitmesi gereken ilanları döner.</summary>
    private async Task<List<JobPosting>> SyncCompanyAsync(
        TrackedCompany company, ScanReport report, CancellationToken ct)
    {
        if (!_sources.TryGetValue(company.Source, out var source))
            throw new InvalidOperationException($"No IJobSource registered for {company.Source}");

        var now = time.GetUtcNow();
        var fetched = await source.FetchAsync(company.Token, ct);
        var existing = (await repository.GetJobsAsync(company.Source, company.Name, ct))
            .ToDictionary(j => j.ExternalId);

        var seen = new HashSet<string>();
        var changed = new List<JobPosting>();

        foreach (var f in fetched)
        {
            if (!seen.Add(f.ExternalId))
                continue; // kaynak aynı ilanı iki kez döndürdüyse

            if (existing.TryGetValue(f.ExternalId, out var job))
            {
                if (job.Refresh(f.Title, f.Location, f.Description, now))
                {
                    changed.Add(job);
                    report.Updated++;
                }
            }
            else
            {
                job = JobPosting.Create(company.Source, f.ExternalId, company.Name,
                    f.Title, f.Location, f.Url, f.Description, now);
                repository.Add(job);
                changed.Add(job);
                report.New++;
            }
        }

        // Güvenlik: kaynak boş döndüyse muhtemelen API sorunu, her şeyi kapatma
        if (fetched.Count == 0 && existing.Values.Any(j => j.Status == JobStatus.Open))
        {
            logger.LogWarning("{Company} returned 0 jobs; skipping close step", company.Name);
        }
        else
        {
            foreach (var job in existing.Values)
            {
                if (!seen.Contains(job.ExternalId) && job.Status == JobStatus.Open)
                {
                    job.Close(now);
                    report.Closed++;
                }
            }
        }

        await repository.SaveChangesAsync(ct);
        return changed;
    }

    private async Task AnalyzeAndMatchAsync(
        JobPosting job, CandidateProfile profile, ScanReport report, CancellationToken ct)
    {
        // Bir ilan patlarsa (LLM saçma cevap verirse) diğerleri devam etsin
        try
        {
            var analysis = await analyzer.AnalyzeAsync(job, ct);
            var match = await matcher.MatchAsync(job, analysis, profile, ct);

            // İkisi de başarılıysa birlikte kaydet — yarım kayıt kalmasın
            repository.AddAnalysis(analysis);
            repository.AddMatch(match);
            await repository.SaveChangesAsync(ct);

            report.Analyzed++;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            report.FailedJobs++;
            logger.LogWarning(ex, "Analyze/match failed for {JobId} '{Title}'", job.Id, job.Title);
        }
    }
}
