# Stop all backend services started from the sos-api folder.
# This stops any dotnet processes whose executable path contains sos-api\src.

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
Set-Location $scriptDir

$processes = Get-Process | Where-Object { $_.Path -and $_.Path -like "*\sos-api\src\*" }

if (-not $processes) {
    Write-Host "No running sos-api services found."
    return
}

$processes | ForEach-Object {
    Write-Host "Stopping: $($_.ProcessName) [Id=$($_.Id)]"
    Stop-Process -Id $_.Id -Force
}

Write-Host "Stopped all matching sos-api services."