# deckify

**Deckify** is a VSTO Add-in for Microsoft PowerPoint designed to streamline deck creation, alignment, and formatting tasks.

## 🚀 Features

See [FEATURES.md](FEATURES.md) for a comprehensive list of tools and capabilities, including:
-   **Shape Alignment**: Advanced batch resizing, distribution, and ratio locking.
-   **Clean Up**: One-click removal of animations, transitions, and metadata.
-   **Productivity**: Sticky notes, stamps, and Harvey balls.

## 🛠️ Prerequisites

-   **OS**: Windows 10/11
-   **Office**: Microsoft PowerPoint 2013 or newer (32-bit or 64-bit)
-   **Framework**: .NET Framework 4.8.1
-   **IDE**: Visual Studio 2022 (Community, Pro, or Enterprise)
    -   *Workload required*: "Office/SharePoint development"

## 🏗️ How to Build

1.  Clone the repository.
2.  Open `Deckify.sln` in Visual Studio 2022.
3.  Restore NuGet packages (should happen automatically).
4.  Build the Solution (`Ctrl+Shift+B` or **Build > Build Solution**).

> **Note**: This is a VSTO Add-in. Building the project automatically registers it with your local PowerPoint installation.

## 🧪 Running Tests

The solution includes a test project `Deckify.Tests` covering service logic and helpers.

**Via Visual Studio**:
1.  Open **Test Explorer** (`Test > Test Explorer`).
2.  Click **Run All**.

**Via CLI**:
```powershell
dotnet test Deckify.Tests/Deckify.Tests.csproj
```

## 📂 Project Structure

-   `Deckify/` - Main VSTO Add-in project.
    -   `Modules/` - Feature modules (Services, Task Panes).
    -   `Common/` - Shared utilities and helpers.
    -   `RibbonMD.xml` - Ribbon UI definition.
-   `Deckify.Tests/` - xUnit test project for unit testing.

## 🤝 Contributing

Please ensure all new logic is covered by unit tests in `Deckify.Tests`.
Run the full test suite before submitting changes.
