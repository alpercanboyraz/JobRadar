using System.Security.Cryptography;
using System.Text;

namespace JobRadar.Domain.Jobs;

public class JobPosting
{
    public Guid Id { get; private set; }

    // Aynı ilanı tanımamızı sağlayan ikili: (Source, ExternalId)
    public JobSourceType Source { get; private set; }
    public string ExternalId { get; private set; } = default!;

    public string CompanyName { get; private set; } = default!;
    public string Title { get; private set; } = default!;
    public string? Location { get; private set; }
    public string Url { get; private set; } = default!;
    public string Description { get; private set; } = default!;

    // İlan değişti mi? Değiştiyse LLM'e tekrar göndereceğiz.
    public string ContentHash { get; private set; } = default!;

    public JobStatus Status { get; private set; }
    public DateTimeOffset FirstSeenAt { get; private set; }
    public DateTimeOffset LastSeenAt { get; private set; }
    public DateTimeOffset? ClosedAt { get; private set; }

    private JobPosting() { } // EF Core bunu kullanacak

    public static JobPosting Create(
        JobSourceType source,
        string externalId,
        string companyName,
        string title,
        string? location,
        string url,
        string description,
        DateTimeOffset now)
    {
        return new JobPosting
        {
            Id = Guid.NewGuid(),
            Source = source,
            ExternalId = externalId,
            CompanyName = companyName,
            Title = title,
            Location = location,
            Url = url,
            Description = description,
            ContentHash = ComputeHash(title, description),
            Status = JobStatus.Open,
            FirstSeenAt = now,
            LastSeenAt = now
        };
    }

    /// <summary>Tekrar görülen ilanı günceller. İçerik değiştiyse true döner.</summary>
    public bool Refresh(string title, string? location, string description, DateTimeOffset now)
    {
        LastSeenAt = now;

        // Kapanmış ilan geri gelirse tekrar aç
        if (Status == JobStatus.Closed)
        {
            Status = JobStatus.Open;
            ClosedAt = null;
        }

        var newHash = ComputeHash(title, description);
        if (newHash == ContentHash)
            return false;

        Title = title;
        Location = location;
        Description = description;
        ContentHash = newHash;
        return true;
    }

    public void Close(DateTimeOffset now)
    {
        if (Status == JobStatus.Closed) return;
        Status = JobStatus.Closed;
        ClosedAt = now;
    }

    private static string ComputeHash(string title, string description)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"{title}\n{description}"));
        return Convert.ToHexString(bytes);
    }
}
