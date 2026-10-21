namespace JobRadar.Application.Abstractions;

/// <summary>Senin CV'ni sağlar. Şimdilik bir dosyadan okuyacağız.</summary>
public interface ICandidateProfileProvider
{
    Task<CandidateProfile> GetAsync(CancellationToken ct);
}

/// <summary>Version: CV değişince değişir (örn. içeriğin hash'i). JobMatch'e yazılır.</summary>
public sealed record CandidateProfile(string Version, string Text);
