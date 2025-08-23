using System.IO;
using System.Text.Json;
using FileSifter.Domain.Config;

namespace FileSifter.Infrastructure.Settings;

public sealed class SettingsService
{
    private readonly string _settingsPath;
    public AppSettings Current { get; private set; } = new();

    public SettingsService(string settingsPath) => _settingsPath = settingsPath;

    public void Load()
    {
        if (!File.Exists(_settingsPath))
        {
            Current = new AppSettings();
            Current.Normalize();
            return;
        }
        try
        {
            var json = File.ReadAllText(_settingsPath);
            var loaded = JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            Current = loaded ?? new AppSettings();
        }
        catch
        {
            Current = new AppSettings();
        }
        Current.Normalize();
    }

    public void Save()
    {
        Current.Normalize();
        var json = JsonSerializer.Serialize(Current, new JsonSerializerOptions { WriteIndented = true });
        Directory.CreateDirectory(Path.GetDirectoryName(_settingsPath)!);
        File.WriteAllText(_settingsPath, json);
    }
}