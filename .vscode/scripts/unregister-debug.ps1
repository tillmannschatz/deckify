$ErrorActionPreference = "Stop"
$registryPath = "HKCU:\Software\Microsoft\Office\PowerPoint\Addins\Deckify.Debug"

if (Test-Path $registryPath) {
    Remove-Item -Path $registryPath -Recurse -Force
    Write-Host "Removed debug registry key: $registryPath"
} else {
    Write-Host "No debug registry key found at $registryPath - nothing to remove."
}
