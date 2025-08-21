namespace FileSifter.Services;

public sealed class ProgressReporter
{
    private readonly object _lock = new();
    private DateTime _lastReport = DateTime.MinValue;

    public void Report(string message, Action<string> sink, int throttleMs = 300)
    {
        lock (_lock)
        {
            var now = DateTime.UtcNow;
            if ((now - _lastReport).TotalMilliseconds < throttleMs) return;
            _lastReport = now;
            sink(message);
        }
    }
}