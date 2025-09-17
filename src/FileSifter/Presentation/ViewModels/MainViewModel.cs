using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Win32;
using FileSifter.App;
using FileSifter.Domain.Config;
using FileSifter.Services;
using FileSifter.Infrastructure.Settings;

namespace FileSifter.Presentation.ViewModels;

public sealed class MainViewModel : INotifyPropertyChanged
{
    private readonly SettingsService _settingsService;
    private readonly AppSettings _settings;
    private CancellationTokenSource? _cts;

    public MainViewModel()
    {
        _settingsService = Bootstrapper.InitializeSettings();
        _settings = _settingsService.Current;

        HashAlgorithms = new[] { "xxhash64", "sha256" };
        ExistingPolicies = new[] { "overwrite", "skip", "rename" };

        SelectedHashAlgorithm = _settings.HashAlgorithm;
        SelectedExistingPolicy = _settings.OnExisting;
        GenerateRemovedList = _settings.GenerateRemovedList;
        OpenExplorerAfterExport = _settings.OpenExplorerAfterExport;
    }

    #region Bindable Properties
    private string _baseFolder = "";
    public string BaseFolder { get => _baseFolder; set { _baseFolder = value; OnChanged(); UpdateCanStart(); } }

    private string _currentFolder = "";
    public string CurrentFolder { get => _currentFolder; set { _currentFolder = value; OnChanged(); UpdateCanStart(); } }

    private string? _exportFolder;
    public string? ExportFolder { get => _exportFolder; set { _exportFolder = value; OnChanged(); } }

    public string[] HashAlgorithms { get; }
    private string _selectedHashAlgorithm = "xxhash64";
    public string SelectedHashAlgorithm { get => _selectedHashAlgorithm; set { _selectedHashAlgorithm = value; OnChanged(); } }

    public string[] ExistingPolicies { get; }
    private string _selectedExistingPolicy = "overwrite";
    public string SelectedExistingPolicy { get => _selectedExistingPolicy; set { _selectedExistingPolicy = value; OnChanged(); } }

    private bool _generateRemovedList;
    public bool GenerateRemovedList { get => _generateRemovedList; set { _generateRemovedList = value; OnChanged(); } }

    private bool _openExplorerAfterExport;
    public bool OpenExplorerAfterExport { get => _openExplorerAfterExport; set { _openExplorerAfterExport = value; OnChanged(); } }

    private double _progress;
    public double Progress { get => _progress; set { _progress = value; OnChanged(); } }

    private string _statusMessage = "Idle";
    public string StatusMessage { get => _statusMessage; set { _statusMessage = value; OnChanged(); } }

    private string _summaryDisplay = "(no run yet)";
    public string SummaryDisplay { get => _summaryDisplay; set { _summaryDisplay = value; OnChanged(); } }

    private bool _canStart;
    public bool CanStart { get => _canStart; set { _canStart = value; OnChanged(); } }

    private bool _canCancel;
    public bool CanCancel { get => _canCancel; set { _canCancel = value; OnChanged(); } }
    #endregion

    #region Commands
    public ICommand BrowseBaseCommand => new RelayCommand(_ =>
    {
        var dlg = new OpenFolderDialog();
        if (dlg.ShowDialog() == true)
            BaseFolder = dlg.FolderName;
    });

    public ICommand BrowseCurrentCommand => new RelayCommand(_ =>
    {
        var dlg = new OpenFolderDialog();
        if (dlg.ShowDialog() == true)
            CurrentFolder = dlg.FolderName;
    });

    public ICommand BrowseExportCommand => new RelayCommand(_ =>
    {
        var dlg = new OpenFolderDialog();
        if (dlg.ShowDialog() == true)
            ExportFolder = dlg.FolderName;
    });

    public ICommand StartCommand => new RelayCommand(async _ => await StartAsync(), _ => CanStart);
    public ICommand CancelCommand => new RelayCommand(_ => _cts?.Cancel(), _ => CanCancel);
    #endregion

    private async Task StartAsync()
    {
        SaveSettingsFromUI();

        _cts = new CancellationTokenSource();
        CanStart = false;
        CanCancel = true;
        Progress = 0;
        StatusMessage = "Working...";

        try
        {
            var diff = new DiffService(_settings);
            var summary = await Task.Run(() => diff.Run(BaseFolder, CurrentFolder, ExportFolder,
                msg => StatusMessage = msg, _cts.Token));

            Progress = 1;
            SummaryDisplay = $"New={summary.Counts.New} Changed={summary.Counts.Changed} Removed={summary.Counts.Removed} " +
                             $"Unchanged={summary.Counts.Unchanged} Errors={summary.Counts.Errors}";

            if (OpenExplorerAfterExport && Directory.Exists(summary.ExportFolder))
                System.Diagnostics.Process.Start("explorer.exe", summary.ExportFolder);

            StatusMessage = "Completed.";
        }
        catch (OperationCanceledException)
        {
            StatusMessage = "Canceled.";
        }
        catch (Exception ex)
        {
            StatusMessage = "Error: " + ex.Message;
        }
        finally
        {
            _cts = null;
            CanStart = true;
            CanCancel = false;
        }
    }

    private void SaveSettingsFromUI()
    {
        _settings.HashAlgorithm = SelectedHashAlgorithm;
        _settings.OnExisting = SelectedExistingPolicy;
        _settings.GenerateRemovedList = GenerateRemovedList;
        _settings.OpenExplorerAfterExport = OpenExplorerAfterExport;
        _settingsService.Save();
    }

    private void UpdateCanStart()
    {
        CanStart = Directory.Exists(BaseFolder) &&
                   Directory.Exists(CurrentFolder) &&
                   (_cts == null);
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    private void OnChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}

public sealed class RelayCommand : ICommand
{
    private readonly Action<object?> _exec;
    private readonly Func<object?, bool>? _can;

    public RelayCommand(Action<object?> exec, Func<object?, bool>? can = null)
    {
        _exec = exec;
        _can = can;
    }

    public bool CanExecute(object? parameter) => _can?.Invoke(parameter) ?? true;
    public void Execute(object? parameter) => _exec(parameter);
    public event EventHandler? CanExecuteChanged
    {
        add => CommandManager.RequerySuggested += value!;
        remove => CommandManager.RequerySuggested -= value!;
    }
}