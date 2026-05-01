$currentPath = $PSScriptRoot
$zipFile = Join-Path $currentPath "deckify-install.zip"
# Extract to a dedicated folder in Temp to avoid path collisions and ensure write access
$tempDir = [System.IO.Path]::GetTempPath()
$destPath = Join-Path $tempDir "deckify_installer_v1"

if (Test-Path $destPath) { Remove-Item -Recurse -Force $destPath }

Write-Host "Extracting deckify..."
try {
    Expand-Archive -Path $zipFile -DestinationPath $destPath -Force
    
    # Check for the publish subfolder if the zip included the parent 'publish' folder
    # Assuming the zip contents were 'setup.exe' etc at root:
    $setupExe = Join-Path $destPath "setup.exe"
    
    # If not found, check if it was zipped with the parent folder 'publish'
    if (-not (Test-Path $setupExe)) {
        $subFolder = Join-Path $destPath "publish"
        if (Test-Path $subFolder) {
            $setupExe = Join-Path $subFolder "setup.exe"
            $destPath = $subFolder # Set working dir to this folder
        }
    }

    if (Test-Path $setupExe) {
        Write-Host "Launching setup from $destPath..."
        # Launch setup.exe and wait for it to start. 
        # Crucial: Set WorkingDirectory to the folder containing setup.exe
        $startInfo = New-Object System.Diagnostics.ProcessStartInfo
        $startInfo.FileName = $setupExe
        $startInfo.WorkingDirectory = $destPath
        $startInfo.UseShellExecute = $true
        [System.Diagnostics.Process]::Start($startInfo)
    }
    else {
        Write-Error "setup.exe not found in extracted archive at $destPath."
        # List contents for debugging
        Get-ChildItem -Recurse $destPath | Select-Object FullName
        Read-Host "Press Enter to exit"
    }
}
catch {
    Write-Error "Error during extraction: $_"
    Read-Host "Press Enter to exit"
}
