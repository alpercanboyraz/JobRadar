namespace JobRadar.Domain.Jobs;

/// <summary>
/// LLM'in bir ilandan çıkardığı yapısal bilgi.
/// İlanın belirli bir içerik sürümüne (ContentHash) bağlıdır.
/// </summary>
public class JobAnalysis
{
    public Guid Id { get; private set; }
    public Guid JobPostingId { get; private set; }

    // Hangi içerik sürümünü analiz ettik? İlan değişirse yeni analiz gerekir.
    public string ContentHash { get; private set; } = default!;
    public string Model { get; private set; } = default!;

    public string[] Skills { get; private set; } = [];
    public Seniority Seniority { get; private set; }
    public string[] LanguageRequirements { get; private set; } = [];
    public WorkMode WorkMode { get; private set; }

    public DateTimeOffset CreatedAt { get; private set; }

    private JobAnalysis() { }

    public static JobAnalysis Create(
        JobPosting job,
        string model,
        IEnumerable<string> skills,
        Seniority seniority,
        IEnumerable<string> languageRequirements,
        WorkMode workMode,
        DateTimeOffset now)
    {
        return new JobAnalysis
        {
            Id = Guid.NewGuid(),
            JobPostingId = job.Id,
            ContentHash = job.ContentHash,
            Model = model,
            Skills = skills.Select(s => s.Trim()).Where(s => s.Length > 0).Distinct().ToArray(),
            Seniority = seniority,
            LanguageRequirements = languageRequirements.ToArray(),
            WorkMode = workMode,
            CreatedAt = now
        };
    }

    public bool IsStaleFor(JobPosting job) => ContentHash != job.ContentHash;
}
