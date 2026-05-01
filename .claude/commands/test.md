---
description: Build and run xUnit tests in Deckify.Tests via dotnet test
allowed-tools: PowerShell, Bash
---

Build and run all unit tests in `tests/Deckify.Tests/Deckify.Tests.csproj`.

Phase 1 introduced `Deckify.Core` (netstandard2.0). The test project references only `Deckify.Core`, not the VSTO project, so the build graph contains no VSTO targets and `dotnet test` works without VS MSBuild.

```powershell
dotnet test tests\Deckify.Tests\Deckify.Tests.csproj --configuration Debug --logger "console;verbosity=normal" --nologo
```

Report passed / failed counts. On failure, summarize the first failed test (name + assertion) — don't dump the entire output.
