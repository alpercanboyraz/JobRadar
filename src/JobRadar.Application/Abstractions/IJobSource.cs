using JobRadar.Domain.Jobs;

namespace JobRadar.Application.Abstractions;

/// <summary>Bir ATS'den ilan çeken adapter. Her ATS için bir implementasyon olacak.</summary>
public interface IJobSource
{
    JobSourceType Type { get; }

    Task<IReadOnlyList<SourceJob>> FetchAsync(string companyToken, CancellationToken ct);
}

/// <summary>ATS'den gelen, tüm kaynaklar için ortak formata çevrilmiş ham ilan.</summary>
public sealed record SourceJob(
    string ExternalId,
    string Title,
    string? Location,
    string Url,
    string Description);
