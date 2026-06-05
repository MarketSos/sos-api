# Start all backend services from sos-api
# This script works from any folder and opens each service in its own console window.

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$projects = @(
    "src/Services/Identity/Sos.Identity.API",
    "src/Services/Catalog/Sos.Catalog.API",
    "src/Services/Inventory/Sos.Inventory.API",
    "src/Services/POS/Sos.POS.API",
    "src/Services/Pricing/Sos.Pricing.API",
    "src/Services/CRM/Sos.CRM.API",
    "src/Services/Loyalty/Sos.Loyalty.API",
    "src/Services/Analytics/Sos.Analytics.API",
    "src/ApiGateway"
)

foreach ($project in $projects) {
    $projectPath = Join-Path $scriptDir $project
    Write-Host "Starting: $projectPath"

    $command = "Set-Location '$scriptDir'; dotnet run --project '$project'"
    Start-Process powershell.exe -ArgumentList '-NoExit', '-Command', $command
}

Write-Host "All services started. Use .\stop-all.ps1 to stop them."