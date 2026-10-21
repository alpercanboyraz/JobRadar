namespace JobRadar.Application.Scanning;

/// <summary>Bir sabah taramasının özeti. Loglama ve ileride bildirim için.</summary>
public sealed class ScanReport
{
    public int New { get; set; }
    public int Updated { get; set; }
    public int Closed { get; set; }
    public int Analyzed { get; set; }
    public int FailedJobs { get; set; }
    public int FailedCompanies { get; set; }

    public override string ToString() =>
        $"new={New} updated={Updated} closed={Closed} analyzed={Analyzed} " +
        $"failedJobs={FailedJobs} failedCompanies={FailedCompanies}";
}
