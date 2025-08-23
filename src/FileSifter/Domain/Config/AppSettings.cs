using System.Collections.Generic;

namespace FileSifter.Domain.Config;

public sealed class AppSettings
{
    public string HashAlgorithm { get; set; } = "xxhash64"; // or sha256
    public string OnExisting { get; set; } = "overwrite";   // overwrite | skip | rename
    public bool GenerateRemovedList { get; set; } = true;
    public bool OpenExplorerAfterExport { get; set; } = true;
    public int Parallelism { get; set; } = 4;

    public List<string>? IncludeExtensions { get; set; }
    public List<RecentPair> RecentPairs { get; set; } = new();

    public HashSet<string> NormalizedExtensions { get; private set; } = new();

    public void Normalize()
    {
        var fallback = new List<string> { ".png", ".pdf", ".jpg", ".jpeg" };
        var source = (IncludeExtensions == null || IncludeExtensions.Count == 0)
            ? fallback
            : IncludeExtensions;

        NormalizedExtensions.Clear();
        foreach (var raw in source)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;
            var ext = raw.Trim();
            if (!ext.StartsWith(".")) ext = "." + ext;
            ext = ext.ToLowerInvariant();
            if (ext.Contains('*') || ext.Contains('?')) continue;
            NormalizedExtensions.Add(ext);
        }
        if (NormalizedExtensions.Count == 0)
            foreach (var fe in fallback) NormalizedExtensions.Add(fe);
    }
}

public sealed class RecentPair
{
    public string Base { get; set; } = string.Empty;
    public string Current { get; set; } = string.Empty;
    public string Timestamp { get; set; } = string.Empty;
}