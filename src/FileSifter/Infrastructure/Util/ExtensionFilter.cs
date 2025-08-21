using FileSifter.Domain.Config;

namespace FileSifter.Infrastructure.Util;

public static class ExtensionFilter
{
    public static bool IsTarget(string path, AppSettings settings)
    {
        var ext = Path.GetExtension(path);
        if (string.IsNullOrEmpty(ext)) return false;
        return settings.NormalizedExtensions.Contains(ext.ToLowerInvariant());
    }
}