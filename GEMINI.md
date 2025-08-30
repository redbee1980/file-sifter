## Project Overview

File-sifter is a WPF application designed to compare two folders (Base and Current) and extract only new or changed files, preserving their original directory structure. It uses a hybrid change detection method (size + mtime, with optional hashing for deeper comparison) and supports various file types. The application generates a summary report in JSON and text formats, and can optionally create a list of removed files. It's built with .NET 8 and focuses on efficient, parallel processing. For more detailed information, please refer to `README.md` and any file in the `docs/` directory whose name starts with `requirements` (e.g., `docs/requirements-v0.7.md`).

## Japanese Version

This `gemini.md` file has a Japanese counterpart, `gemini.ja.md`. Please ensure that `gemini.ja.md` is updated whenever this file is modified.

## Project-specific Conventions

*   **Coding Style**: Follow the existing codebase's style.
*   **Naming Conventions**: Follow the existing codebase's naming conventions.
*   **Architectural Patterns**: Follow the existing codebase's architectural patterns (e.g., MVVM).
*   **Testing Philosophy**: Follow the existing codebase's testing philosophy.
*   **Versioning**: This project adheres to [Semantic Versioning](https://semver.org/) (vMAJOR.MINOR.PATCH).

## Preferred Tools and Commands

*   **Build**: `dotnet build`
*   **Test**: `dotnet test` (if applicable)
*   **Linting/Code Analysis**: Follow existing practices (if applicable).
*   **Dependency Management**: `dotnet restore`

## Common Workflows

*   **Feature Development**: Create a new branch, implement, test, commit, push.
*   **Bug Fixing**: Reproduce the bug, write a test, fix, test, commit, push.
*   **Deployment**: (To be defined)

## Important Files/Directories

*   **Configuration Files**:
    *   `settings.sample.json`: Provides a template for available application settings.
    *   `FileSifter.csproj`: Defines project properties, dependencies, and build process.
*   **Sensitive Information**: Never expose, log, or commit any sensitive information, API keys, or credentials.
*   **Areas to avoid modifying**:
    *   `bin/` and `obj/`: Auto-generated output directories for compiled binaries and intermediate build files.
    *   `.vs/`: Visual Studio internal files.
    *   `serena/`: An embedded git repository; treat as an external dependency and avoid direct modification.

## AI Agent Specific Instructions

*   **Verbosity**: Be concise unless asked for details.
*   **Confirmation**: Always confirm before making significant changes to the codebase. Explain the command's purpose and potential impact before execution.
*   **Language**: Prefer Japanese for responses.
