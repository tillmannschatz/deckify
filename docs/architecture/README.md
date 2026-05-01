# Architecture Documentation

## Overview
**Deckify** is a VSTO (Visual Studio Tools for Office) Add-in for Microsoft PowerPoint. It enhances productivity by providing advanced shape manipulation, color management, and smart mirroring features.

## Technical Stack
- **Framework**: .NET Framework 4.8.1
- **Platform**: VSTO (PowerPoint 2013+)
- **Language**: C# 12.0
- **UI**: Windows Forms (Task Panes) & Ribbon XML
- **Dependency Injection**: `Microsoft.Extensions.DependencyInjection`
- **JSON Parsing**: `Newtonsoft.Json`
- **Logging**: `System.Diagnostics.Trace` (File-based)

## Core Architecture

### Dependency Injection (DI)
The application uses a Service Locator pattern anchored in `ThisAddIn`. Services are registered in `ThisAddIn.ConfigureServices` and resolved via `Globals.ThisAddIn.ServiceProvider`.

**Key Services:**
- `IShapeService`: Core logic for shape manipulation (sizing, spacing, alignment).
- `IPaletteService`: Manages corporate color palettes and grouping.
- `IMirrorService`: Handles "Smart Mirror" functionality (tagging and text sync).
- `IUpdateService`: Checks for updates via GitHub Releases.
- `IEmailService`: Handles Outlook integration for feedback.
- `IReferenceService`: Manages slide references/links.

### Project Structure
```
Deckify/
├── Common/              # Shared utilities (Logger, VersionHelpers, ErrorHandler)
├── Modules/             # Feature-specific logic
│   ├── AboutManager/    # Update logic & About Pane
│   ├── Colors/          # Color Palette feature
│   ├── Mirror/          # Smart Mirror logic
│   ├── ScreenshotManager/ # Screenshot tools
│   ├── Services/        # Core business logic services
│   └── SettingsManager/ # Configuration Pane
├── RibbonMD.xml         # Ribbon UI definition
├── RibbonMD.cs          # Ribbon Callbacks (Controller)
└── ThisAddIn.cs         # Entry Point & Service Configuration
```

## Key Flows

### 1. Startup
1. `ThisAddIn_Startup` is triggered.
2. `ConfigureServices()` initializes the DI container.
3. Services like `MirrorService` are instantiated and hook into Application events (`WindowSelectionChange`).
4. Task Panes are initialized lazily per Window via `EnsureTaskPanesInitializedForWindow`.

### 2. Update Mechanism
- **Trigger**: User clicks "Check for Updates" in the About Pane.
- **Logic**: `UpdateService` fetches the latest release from the GitHub API.
- **Versioning**: Compares the *installed* ClickOnce version (found via `VersionHelpers.GetInstalledVersion()`) against the remote tag.
- **Action**: If newer, prompts user to download the installer assets.

### 3. Dynamic Ribbon
- `ThisAddIn.Application_WindowSelectionChange` detects selection changes.
- Calls `DynamicGroupActivateDeactivate`.
- Uses `ShapeService.CheckActiveShapeType` to determine context (Table, Picture).
- Invalidates specific Ribbon groups to show/hide context-sensitive tools.

### 4. Smart Mirror
- **Copy**: `MirrorService` generates a GUID tag on the source shape.
- **Paste**: Checks clipboard for tagged shape; if found, links the new shape to the same ID.
- **Sync**: `WindowSelectionChange` detects when a tracked shape is deselected. `CheckAndSyncPreviousShape` pushes text changes to all other shapes with the same ID.

## Data Persistence
- **User Settings**: Stored in `Properties.Settings.Default` (user-scoped).
- **Color Palettes**: Serialized to JSON and stored in a user-specific file (managed by `PaletteService`).
