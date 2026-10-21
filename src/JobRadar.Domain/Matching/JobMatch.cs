using JobRadar.Domain.Jobs;

namespace JobRadar.Domain.Matching;

/// <summary>
/// Bir ilanın belirli bir CV sürümüne, belirli bir modelle verilen uyum skoru.
/// Aynı ilan için birden fazla JobMatch olabilir (farklı CV sürümü / model).
/// </summary>
public class JobMatch
{
    public Guid Id { get; private set; }
    public Guid JobPostingId { get; private set; }

    public string CvVersion { get; private set; } = default!;
    public string Model { get; private set; } = default!;

    public int Score { get; private set; }               // 0–100
    public string Reasoning { get; private set; } = default!;
    public string[] MatchedSkills { get; private set; } = [];
    public string[] MissingSkills { get; private set; } = [];

    public DateTimeOffset CreatedAt { get; private set; }

    private JobMatch() { }

    public static JobMatch Create(
        JobPosting job,
        string cvVersion,
        string model,
        int score,
        string reasoning,
        IEnumerable<string> matchedSkills,
        IEnumerable<string> missingSkills,
        DateTimeOffset now)
    {
        // LLM'e güvenme: sınırları domain'de zorla
        if (score is < 0 or > 100)
            throw new ArgumentOutOfRangeException(nameof(score), score, "Score must be 0–100.");
        if (string.IsNullOrWhiteSpace(reasoning))
            throw new ArgumentException("Reasoning is required.", nameof(reasoning));

        return new JobMatch
        {
            Id = Guid.NewGuid(),
            JobPostingId = job.Id,
            CvVersion = cvVersion,
            Model = model,
            Score = score,
            Reasoning = reasoning.Trim(),
            MatchedSkills = matchedSkills.ToArray(),
            MissingSkills = missingSkills.ToArray(),
            CreatedAt = now
        };
    }
}
