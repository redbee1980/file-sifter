# Repository Guidelines

This guide summarizes how to work effectively in the file-sifter repository.

## Project Structure & Module Organization

- src/FileSifter: WPF (.NET 8) application root (primary project).
- src/FileSifter/App, Presentation: app startup, UI and window composition.
- Domain, Services, Infrastructure: core logic, orchestration, and IO/OS integrations.
- docs: additional guidelines (e.g., docs/commits.md for commit rules).
- settings.json: generated at runtime under `%APPDATA%/FileSifter/settings.json`.

## Build, Test, and Development Commands

```bash
# Build & run (WPF)
dotnet build
dotnet run --project src/FileSifter

# Build specific project
cd src/FileSifter && dotnet build

# (Planned) tests
dotnet test    # once tests/ are added
```

## Coding Style & Naming Conventions

- Language: C# 10+, .NET 8, nullable enabled, implicit usings.
- Indentation: 4 spaces; keep methods focused and small.
- Naming: PascalCase for public types/members; `_camelCase` for private fields; `var` when RHS type is obvious.
- Structure: prefer small, single-purpose classes; split >150-line classes.
- Formatting: align with forthcoming `.editorconfig`; optionally use `dotnet format` locally.

## Testing Guidelines

- Framework: xUnit or NUnit (TBD); target `net8.0`.
- Location: `tests/FileSifter.Tests/` (planned); mirror namespaces of `src/FileSifter`.
- Names: `ClassName_Method_Scenario_Expected()`; arrange/act/assert pattern.
- Run: `dotnet test` from repo root once tests exist; aim for meaningful coverage on Services/Domain.

## Commit & Pull Request Guidelines

- Commits: follow Conventional Commits (see `docs/commits.md`). Examples:
  - `feat: add progress estimation model`
  - `fix: handle empty includeExtensions gracefully`
- Branches: `feature/*`, `fix/*`, `docs/*`, `chore/*`; PRs target `main`.
- PRs: include summary, context, screenshots for UI changes, and clear test/validation steps. Keep 1 PR = 1 topic.

## Security & Configuration Tips

- Do not commit secrets or machine-specific paths; review diffs for `%APPDATA%` artifacts.
- Configuration lives in `%APPDATA%/FileSifter/settings.json`; document changes in README if fields are added/renamed.
- Large file operations: prefer streaming and cancellation-safe patterns (already used); avoid blocking UI thread in Presentation.

## エージェント向け指示

- 返答言語: 原則「日本語」。短く具体的に、必要十分な説明を心がける。
- コマンドやパスはコード体裁（`...`）で示す。英語用語は原語を併記して可。
- Issue/PR の説明・レビューコメントも日本語で可。ユーザー入力が英語の場合のみ英語対応を検討。
