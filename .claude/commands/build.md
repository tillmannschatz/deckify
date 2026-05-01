---
description: Build Deckify.sln in Debug configuration via MSBuild
allowed-tools: PowerShell, Bash
---

Build the Deckify solution in Debug configuration.

The VSTO project (`src/Deckify/Deckify.csproj`) imports `Microsoft.VisualStudio.Tools.Office.targets`, which only ships with Visual Studio's MSBuild — so `dotnet build` cannot build it. We discover VS's MSBuild via `vswhere` and run `Restore;Build` so that both packages.config (legacy) and PackageReference (Core, Tests) are restored in one pass.

```powershell
$installPath = & "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath
$msbuild = Join-Path $installPath "MSBuild\Current\Bin\MSBuild.exe"
& $msbuild Deckify.sln -t:Restore`;Build -p:Configuration=Debug -p:RestorePackagesConfig=true -v:minimal -nologo
```

Report the outcome concisely:
- On success: confirm Build succeeded, mention warning count if non-zero.
- On failure: surface the first error so the user can act on it. Don't dump the entire log.
