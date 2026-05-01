param(
    [string]$Configuration = "Debug"
)
$ErrorActionPreference = "Stop"

$installPath = & "C:\Program Files (x86)\Microsoft Visual Studio\Installer\vswhere.exe" -latest -property installationPath
if (-not $installPath) { Write-Error "Visual Studio installation not found via vswhere."; exit 1 }
$msbuild = Join-Path $installPath "MSBuild\Current\Bin\MSBuild.exe"
if (-not (Test-Path $msbuild)) { Write-Error "MSBuild.exe not found at $msbuild"; exit 1 }

# Restore both packages.config (legacy VSTO project) and PackageReference
# (Deckify.Core, Deckify.Tests) in one pass, then build the whole solution.
& $msbuild Deckify.sln -t:Restore`;Build -p:Configuration=$Configuration -p:RestorePackagesConfig=true -v:minimal -nologo
exit $LASTEXITCODE
