namespace FileSifter.Domain.Models;

public record FileMeta(
    string RelativePath,
    string FullPath,
    long Size,
    DateTime MTimeUtc,
    ulong? XxHash64,
    byte[]? Sha256);