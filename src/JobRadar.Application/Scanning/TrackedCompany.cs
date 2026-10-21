using JobRadar.Domain.Jobs;

namespace JobRadar.Application.Scanning;

/// <summary>Takip ettiğimiz şirket. Token = ATS'deki şirket adı (örn. boards.greenhouse.io/{token}).</summary>
public sealed record TrackedCompany(string Name, JobSourceType Source, string Token);
