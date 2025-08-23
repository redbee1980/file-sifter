using System.IO;
using System.Text;
using System.Text.Json;
using FileSifter.Domain.Results;

namespace FileSifter.Infrastructure.Export;

public static class SummaryWriter
{
    public static void Write(string folder, SummaryModel model)
    {
        Directory.CreateDirectory(folder);

        var jsonPath = Path.Combine(folder, "summary.json");
        var json = JsonSerializer.Serialize(model, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(jsonPath, json, Encoding.UTF8);

        var txtPath = Path.Combine(folder, "summary.txt");
        File.WriteAllText(txtPath,
$"""
Base: {model.BaseFolder}
Current: {model.CurrentFolder}
Export: {model.ExportFolder}
HashAlgorithm: {model.HashAlgorithm}
Extensions: {string.Join(", ", model.TargetExtensions)}
New: {model.Counts.New}
Changed: {model.Counts.Changed}
Removed: {model.Counts.Removed}
Unchanged: {model.Counts.Unchanged}
Errors: {model.Counts.Errors}
Duration(ms): {model.Timing.DurationMs}
""", Encoding.UTF8);
    }
}