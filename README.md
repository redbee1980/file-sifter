# file-sifter

Sift out only new or changed files between two folders (Base vs Current) and export them with the original directory structure preserved.

> MVP: Binary level comparison (size + mtime + optional hash) for `.png .pdf .jpg .jpeg` (config extensible). No visual diff, no PDF rendering.

## Features (MVP)

- Recursive scan (Base / Current)
- Hybrid change detection: size+mtime quick check → hash (xxHash64 / SHA-256) only when needed
- Classification: New / Changed / Unchanged / Removed / Error
- Export only New + Changed files (structure preserved)
- Optional removed list (removed.txt)
- Summary JSON + TXT
- Parallel hashing & progress
- Cancel support (atomic temporary folder rollback)
- Config via `settings.json` (no UI for extension list yet)

## Non-Goals (MVP)

Visual diff, image rendering, PDF page raster, delta visualization, HTML report, CLI, semantic diff, caching.

## Getting Started

### Prerequisites
- Windows 10/11 x64
- .NET 8 SDK
- (Optional) Visual Studio 2022 / Rider / VS Code (C# Dev Kit)

### Build

```bash
git clone https://github.com/redbee1980/file-sifter.git
cd file-sifter/src/FileSifter
dotnet build
dotnet run
```

(実行すると WPF ウィンドウが起動)

### Typical Workflow
1. Launch app.
2. Select Base folder (previous snapshot).
3. Select Current folder (latest assets).
4. Choose: hash algorithm (default xxHash64), existing file policy, removed list option.
5. Optional: specify export folder (or leave blank to auto-name).
6. Press Start → wait for progress.
7. Explorer opens automatically with exported new/changed files.

### Configuration

A sample settings file is created on first run in:
`%APPDATA%/FileSifter/settings.json`

Default content:

```json
{
  "hashAlgorithm": "xxhash64",
  "onExisting": "overwrite",
  "generateRemovedList": true,
  "openExplorerAfterExport": true,
  "parallelism": 4,
  "includeExtensions": [".png", ".pdf", ".jpg", ".jpeg"],
  "recentPairs": []
}
```

Edit to add more extensions (e.g. ".json", ".docx").  
Leave `includeExtensions` empty to revert to defaults.

### Export Output Structure

```
<ExportRoot>/
  summary.json
  summary.txt
  (removed.txt)   # if enabled
  <relative paths of new + changed files...>
```

### Summary JSON Example

```json
{
  "base": "D:/assets/2025-07",
  "current": "D:/assets/2025-08",
  "export": "E:/output/2025-08_vs_2025-07",
  "hashAlgorithm": "xxhash64",
  "targetExtensions": [".png", ".pdf", ".jpg", ".jpeg"],
  "counts": { "new": 20, "changed": 35, "removed": 15, "unchanged": 505, "errors": 0 },
  "timing": { "startedUtc": "2025-08-21T12:00:00Z", "endedUtc": "2025-08-21T12:03:15Z", "durationMs": 195000 }
}
```

## Roadmap (Short)

| Phase | Item |
|-------|------|
| 1 | Core scanning & classification |
| 2 | Parallel hash + progress + cancel |
| 3 | Copy + atomic temp folder |
| 4 | Summary + removed + errors |
| 5 | Settings persistent + recent pairs |
| 6 | Polish (logging, rename policy) |

Backlog (Later): extension UI, exclude patterns, hash cache, semantic diff, CLI, HTML report.

## Development Structure

```
src/FileSifter/
  FileSifter.csproj
  App/
  Domain/
  Infrastructure/
  Services/
  Presentation/
  docs/
```

## Contributing

(Internal MVP — open contribution rules TBD)

## License

MIT (see LICENSE)

## Git Commands (Initial Push)

```bash
# After creating empty repo on GitHub:
git init
git remote add origin git@github.com:redbee1980/file-sifter.git
git add .
git commit -m "feat: initial scaffold (MVP skeleton)"
git push -u origin main
```

## Notes

- Hybrid mode can miss ultra-minor binary differences if size+mtime unchanged (expected trade-off). Use `sha256` + forced hash-only in a future version if necessary.
- JPEG re-encodes will appear as Changed even if visually identical.

Enjoy fast incremental export!