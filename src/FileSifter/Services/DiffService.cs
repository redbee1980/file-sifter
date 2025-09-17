using FileSifter.Domain.Config;
using FileSifter.Domain.Models;
using FileSifter.Domain.Results;
using FileSifter.Infrastructure.Scanning;
using System.IO;

namespace FileSifter.Services;

public sealed class DiffService
{
    private readonly AppSettings _settings;
    private readonly HashService _hashService;
    private readonly CopyService _copyService;
    private readonly ProgressReporter _progress;

    public DiffService(AppSettings settings)
    {
        _settings = settings;
        _hashService = new HashService(settings.HashAlgorithm);
        _copyService = new CopyService();
        _progress = new ProgressReporter();
    }

    public SummaryModel Run(string baseDir, string currentDir, string? exportDir, Action<string> log, CancellationToken token)
    {
        var start = DateTime.UtcNow;

        if (!Directory.Exists(baseDir)) throw new DirectoryNotFoundException(baseDir);
        if (!Directory.Exists(currentDir)) throw new DirectoryNotFoundException(currentDir);

        log("Scanning...");
        var scanner = new FileScanner(_settings);
        var baseFiles = scanner.EnumerateTargetFiles(baseDir).ToList();
        var currFiles = scanner.EnumerateTargetFiles(currentDir).ToList();

        var baseMap = baseFiles.ToDictionary(r => r);
        var currMap = currFiles.ToDictionary(r => r);

        var all = baseMap.Keys.Union(currMap.Keys).OrderBy(p => p).ToList();

        // Collect metadata (size + mtime)
        var metaBase = new Dictionary<string, (long size, DateTime mtime)>();
        foreach (var r in baseMap.Keys)
        {
            var full = Path.Combine(baseDir, r);
            var fi = new FileInfo(full);
            metaBase[r] = (fi.Length, fi.LastWriteTimeUtc);
        }
        var metaCurr = new Dictionary<string, (long size, DateTime mtime)>();
        foreach (var r in currMap.Keys)
        {
            var full = Path.Combine(currentDir, r);
            var fi = new FileInfo(full);
            metaCurr[r] = (fi.Length, fi.LastWriteTimeUtc);
        }

        var results = new List<DiffResult>();
        // Determine candidates needing hash
        var hashCandidates = new List<string>();

        foreach (var rel in all)
        {
            token.ThrowIfCancellationRequested();
            var inBase = metaBase.TryGetValue(rel, out var mb);
            var inCurr = metaCurr.TryGetValue(rel, out var mc);

            if (!inBase && inCurr)
            {
                results.Add(new DiffResult(rel, FileChangeStatus.New, null));
                continue;
            }
            if (inBase && !inCurr)
            {
                results.Add(new DiffResult(rel, FileChangeStatus.Removed, null));
                continue;
            }
            // both exist
            if (mb.size == mc.size && mb.mtime == mc.mtime)
            {
                results.Add(new DiffResult(rel, FileChangeStatus.Unchanged, null));
            }
            else
            {
                hashCandidates.Add(rel);
            }
        }

        // Hash compare for candidates
        if (hashCandidates.Count > 0)
        {
            log($"Hashing {hashCandidates.Count} candidates...");
            Parallel.ForEach(hashCandidates, new ParallelOptions
            {
                MaxDegreeOfParallelism = _settings.Parallelism > 0 ? _settings.Parallelism : Environment.ProcessorCount / 2
            }, rel =>
            {
                token.ThrowIfCancellationRequested();
                try
                {
                    var basePath = Path.Combine(baseDir, rel);
                    var currPath = Path.Combine(currentDir, rel);
                    var (bx, bs) = _hashService.Compute(basePath);
                    var (cx, cs) = _hashService.Compute(currPath);

                    bool changed = false;
                    if (_settings.HashAlgorithm == "xxhash64")
                        changed = bx != cx;
                    else
                        changed = !bs!.SequenceEqual(cs!);

                    lock (results)
                    {
                        results.Add(new DiffResult(rel, changed ? FileChangeStatus.Changed : FileChangeStatus.Unchanged, null));
                    }
                }
                catch (Exception ex)
                {
                    lock (results)
                    {
                        results.Add(new DiffResult(rel, FileChangeStatus.Error, ex.Message));
                    }
                }
            });
        }

        token.ThrowIfCancellationRequested();

        // Export
        var exportRoot = exportDir;
        if (string.IsNullOrWhiteSpace(exportRoot))
        {
            exportRoot = Path.Combine(
                Directory.GetParent(currentDir)!.FullName,
                $"{Path.GetFileName(currentDir)}_vs_{Path.GetFileName(baseDir)}");
        }

        var temp = Path.Combine(exportRoot, ".tmp_session_" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(temp);

        var toCopy = results.Where(r => r.Status is FileChangeStatus.New or FileChangeStatus.Changed).ToList();
        log($"Copying {toCopy.Count} files...");
        foreach (var r in toCopy)
        {
            token.ThrowIfCancellationRequested();
            var src = Path.Combine(currentDir, r.RelativePath);
            var dest = Path.Combine(temp, r.RelativePath);
            try
            {
                _copyService.CopyFile(src, dest, _settings.OnExisting);
            }
            catch (Exception ex)
            {
                results.Add(new DiffResult(r.RelativePath, FileChangeStatus.Error, ex.Message));
            }
        }

        token.ThrowIfCancellationRequested();

        // Promote temp
        Directory.CreateDirectory(exportRoot);
        MoveContents(temp, exportRoot);
        if (Directory.Exists(temp))
            Directory.Delete(temp, true);

        // removed.txt
        if (_settings.GenerateRemovedList)
        {
            var removed = results.Where(r => r.Status == FileChangeStatus.Removed).Select(r => r.RelativePath);
            File.WriteAllLines(Path.Combine(exportRoot, "removed.txt"), removed);
        }

        // summary
        var summary = new SummaryModel
        {
            BaseFolder = baseDir,
            CurrentFolder = currentDir,
            ExportFolder = exportRoot,
            HashAlgorithm = _settings.HashAlgorithm,
            TargetExtensions = _settings.NormalizedExtensions.ToArray(),
            Counts = new()
            {
                New = results.Count(r => r.Status == FileChangeStatus.New),
                Changed = results.Count(r => r.Status == FileChangeStatus.Changed),
                Removed = results.Count(r => r.Status == FileChangeStatus.Removed),
                Unchanged = results.Count(r => r.Status == FileChangeStatus.Unchanged),
                Errors = results.Count(r => r.Status == FileChangeStatus.Error)
            },
            Timing = new()
            {
                StartedUtc = start,
                EndedUtc = DateTime.UtcNow
            }
        };
        Infrastructure.Export.SummaryWriter.Write(exportRoot, summary);

        log("Done.");
        return summary;
    }

    private static void MoveContents(string sourceDir, string targetDir)
    {
        foreach (var dir in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(sourceDir, dir);
            Directory.CreateDirectory(Path.Combine(targetDir, rel));
        }
        foreach (var file in Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories))
        {
            var rel = Path.GetRelativePath(sourceDir, file);
            var dest = Path.Combine(targetDir, rel);
            Directory.CreateDirectory(Path.GetDirectoryName(dest)!);
            if (File.Exists(dest)) File.Delete(dest);
            File.Move(file, dest);
        }
    }
}