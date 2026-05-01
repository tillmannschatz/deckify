$ErrorActionPreference = "Stop"

# Side-by-Side: Debug-Build wird unter eigener VSTO-Identity registriert
# (Deckify.Debug), sodass es parallel zur Production-ClickOnce-Installation
# (Identity "Deckify") in PowerPoint geladen werden kann.
$registryPath = "HKCU:\Software\Microsoft\Office\PowerPoint\Addins\Deckify.Debug"

# Get absolute path to the VSTO manifest in bin\Debug
$scriptPath = $PSScriptRoot
$manifestPath = "file:///$scriptPath/src/Deckify/bin/Debug/Deckify.Debug.vsto|vstolocal"

Write-Host "Configuring VSTO Add-in for Debugging (Side-by-Side)..."
Write-Host "Registry Path: $registryPath"
Write-Host "Manifest Path: $manifestPath"

# Clean existing key to ensure no conflicts with installed versions
if (Test-Path $registryPath) {
    Remove-Item -Path $registryPath -Force
    Write-Host "Removed existing registry key."
}

# Create new key
New-Item -Path $registryPath -Force | Out-Null
New-ItemProperty -Path $registryPath -Name "Description" -Value "deckify Debug Build (Side-by-Side)" -PropertyType String | Out-Null
New-ItemProperty -Path $registryPath -Name "FriendlyName" -Value "deckify-debug" -PropertyType String | Out-Null
New-ItemProperty -Path $registryPath -Name "LoadBehavior" -Value 3 -PropertyType DWord | Out-Null
New-ItemProperty -Path $registryPath -Name "Manifest" -Value $manifestPath -PropertyType String | Out-Null

Write-Host "Successfully registered VSTO manifest."
Write-Host "---------------------------------------------------"
Write-Host "INSTRUCTIONS:"
Write-Host "1. Close PowerPoint."
Write-Host "2. Run a Debug build (e.g. .vscode/scripts/build.ps1 -Configuration Debug)."
Write-Host "3. Open PowerPoint."
Write-Host "The Debug-Add-in (Tab 'deckify-debug') laedt parallel zu einer ggf."
Write-Host "installierten Production-Version (Tab 'deckify')."
Write-Host "---------------------------------------------------"
