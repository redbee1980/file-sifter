namespace FileSifter.Domain.Results;

public sealed class SummaryModel
{
    public string BaseFolder { get; set; } = "";
    public string CurrentFolder { get; set; } = "";
    public string ExportFolder { get; set; } = "";
    public string HashAlgorithm { get; set; } = "";
    public string[] TargetExtensions { get; set; } = Array.Empty<string>();
    public CountsModel Counts { get; set; } = new();
    public TimingModel Timing { get; set; } = new();
}

public sealed class CountsModel
{
    public int New { get; set; }
    public int Changed { get; set; }
    public int Removed { get; set; }
    public int Unchanged { get; set; }
    public int Errors { get; set; }
}

public sealed class TimingModel
{
    public DateTime StartedUtc { get; set; }
    public DateTime EndedUtc { get; set; }
    public long DurationMs => (long)(EndedUtc - StartedUtc).TotalMilliseconds;
}