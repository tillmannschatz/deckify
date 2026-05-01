# Contributing to Deckify

Thank you for your interest in contributing to the **deckify** (Deckify) VSTO Add-in! This document provides guidelines and instructions for setting up your development environment.

## Prerequisites

To build and run this project, you need:
- **Visual Studio 2022** (Community, Professional, or Enterprise)
- **.NET Framework 4.8.1 Developer Pack**
- **Visual Studio Tools for Office (VSTO)** workload
- **Microsoft PowerPoint** (2013, 2016, 2019, or 365) installed locally

## Getting Started

1. **Clone the Repository**:
   ```bash
   git clone https://github.com/tillmannschatz/deckify.git
   ```
2. **Open the Solution**:
   - Open `Deckify.sln` in Visual Studio 2022.
3. **Restore NuGet Packages**:
   - Right-click the Solution in Solution Explorer -> **Restore NuGet Packages**.
4. **Build**:
   - Build the solution (Ctrl+Shift+B). Ensure there are no errors.

## Development Workflow

### Running the Add-in
1. Set the build configuration to **Debug**.
2. Press **F5** or click **Start**.
3. PowerPoint will launch with the add-in loaded.
4. The add-in features are located in the customized Ribbon tab "deckify" (Production) or "deckify-debug" (Debug-Build, parallel installierbar).

### Debugging
- Breakpoints can be set in C# code.
- `System.Diagnostics.Trace.WriteLine` output appears in the Visual Studio Output window.
- Log files are written to `%TEMP%\deckify_Log.txt` (see `Deckify.Common.Logger.LogFileName`).

### Adding a New Feature
1. **Plan**: Identify the module where the feature belongs (e.g., `Modules\Colors` or `Modules\Services`).
2. **Implement**:
   - If adding a Service, define an Interface (`IService`) and Implementation (`Service`).
   - Register the service in `ThisAddIn.ConfigureServices`.
3. **UI**:
   - If adding a Ribbon button, update `RibbonMD.xml` and handle the callback in `RibbonMD.cs`.
   - If adding a Task Pane, create a `UserControl` in the appropriate Module.

## Coding Guidelines

- **Style**: Standard C# conventions.
- **Async**: Use `async/await` for network calls (e.g., Updates) to avoid freezing the UI.
- **Exceptions**: Never swallow exceptions silently. Use `Deckify.Common.Logger.LogError` or `ErrorHandler.ExecuteSafe`.
- **Dependency Injection**: Avoid static service access where possible. Inject `IServiceProvider` or specific services.

## Testing

- **Unit Tests**: Located in `Deckify.Tests` (if available). Run via Test Explorer.
- **Manual Verification**: Always verify Ribbon interactions and Task Pane behavior in PowerPoint before committing.
