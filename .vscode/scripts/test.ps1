$ErrorActionPreference = "Stop"

# Phase 1 entkoppelte das Test-Projekt vom VSTO-Hauptprojekt: Deckify.Tests
# referenziert nur noch Deckify.Core (netstandard2.0). Damit bringt der Build-
# Graph keine VSTO-Targets mehr ein, und `dotnet test` funktioniert direkt.
dotnet test tests\Deckify.Tests\Deckify.Tests.csproj `
    --configuration Debug `
    --logger "console;verbosity=normal" `
    --nologo

exit $LASTEXITCODE
