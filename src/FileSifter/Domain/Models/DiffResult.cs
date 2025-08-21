namespace FileSifter.Domain.Models;

public enum FileChangeStatus
{
    New,
    Changed,
    Unchanged,
    Removed,
    Error
}

public record DiffResult(
    string RelativePath,
    FileChangeStatus Status,
    string? ErrorMessage);