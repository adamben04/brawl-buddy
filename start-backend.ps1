Write-Host "Starting Brawl Buddy Backend..." -ForegroundColor Green
Write-Host ""

Set-Location "BrawlBuddy.Api"
Write-Host "Current directory: $(Get-Location)" -ForegroundColor Yellow
Write-Host ""

Write-Host "Running: dotnet run --urls=`"http://localhost:5001`"" -ForegroundColor Cyan
try {
    dotnet run --urls="http://localhost:5001"
} catch {
    Write-Host "Error starting backend: $_" -ForegroundColor Red
}

Write-Host ""
Write-Host "Backend stopped. Press any key to continue..." -ForegroundColor Yellow
Read-Host
