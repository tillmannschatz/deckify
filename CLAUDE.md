# deckify

VSTO add-in for Microsoft PowerPoint. Streamlines deck creation, shape alignment, and formatting.

## Stack
.NET Framework 4.8.1, C# 12, VSTO 4.0, WinForms task panes, Ribbon XML, `Microsoft.Extensions.DependencyInjection`, xUnit. Distributed via ClickOnce.

## Build / Test
- Build: `/build` (or `MSBuild.exe Deckify.sln -p:Configuration=Debug`)
- Test: `/test` (or `dotnet test Deckify.Tests/Deckify.Tests.csproj`)

## Hard rules
- **No new NuGet packages** without asking first.
- **Do not migrate to .NET 6+** — VSTO requires .NET Framework.
- **COM access must stay on the main Office/UI thread.** Background work must extract primitive IDs (slide index, shape name, tag value) on the UI thread, process those off-thread, and re-resolve COM objects when marshaling back.
- **Do not change deployment/installer settings** (signing, manifests, ClickOnce config) unless explicitly requested.
- **No silent exception handling.** Use `Deckify.Common.Logger` and wrap UI/Ribbon entry points with `Deckify.Common.ErrorHandler.SafeExecute`.

## Where to look
- **Architecture:** [docs/architecture/README.md](docs/architecture/README.md)
- **Project state, COM hotspots, modernization roadmap, manual test checklist:** [KNOWLEDGE.md](KNOWLEDGE.md)
- **User-facing features:** [USER_GUIDE.md](USER_GUIDE.md), [FEATURES.md](FEATURES.md)
- **Setup, build, contribute, deploy:** [README.md](README.md), [CONTRIBUTING.md](CONTRIBUTING.md), [DEPLOYMENT.md](DEPLOYMENT.md)

## Auto-loading skills
Two project skills load on demand based on triggers:
- `vsto-powerpoint` — VSTO/Ribbon/Task-Pane/Office-Interop code
- `com-interop-discipline` — `Marshal.ReleaseComObject`, `Microsoft.Office.Interop.*`, COM property chains

> **Modernization in progress.** The repo is migrating toward a target architecture (`src/`-layout, dedicated Interop layer, ComScope discipline, Serilog logging, WPF task panes). Existing code is mid-migration. See [KNOWLEDGE.md](KNOWLEDGE.md) for the phase plan and current status.
