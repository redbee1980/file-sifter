using FileSifter.Domain.Config;

namespace FileSifter.Infrastructure.Scanning;

public sealed class FileScanner
{
    private readonly AppSettings _settings;

    public FileScanner(AppSettings settings) => _settings = settings;

    public IEnumerable<string> EnumerateTargetFiles(string root)
    {
        if (!Directory.Exists(root)) yield break;
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var ext = Path.GetExtension(file).ToLowerInvariant();
            if (!_settings.NormalizedExtensions.Contains(ext)) continue;
            yield return Path.GetRelativePath(root, file);
        }
    }
}