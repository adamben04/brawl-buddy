# Test specific player tag
$TestPlayerTag = "#P9VQPV9YC"
$baseUrl = "http://localhost:5000/api"

Write-Host "Testing player search for: $TestPlayerTag" -ForegroundColor Yellow

# First check if backend is running
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/brawler" -Method Get -TimeoutSec 5
    Write-Host "✅ Backend is running!" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend is not running. Please start it first." -ForegroundColor Red
    Write-Host "Run: cd BrawlBuddy.Api; dotnet run" -ForegroundColor Gray
    exit 1
}

# Test the specific player
$EncodedPlayerTag = [System.Web.HttpUtility]::UrlEncode($TestPlayerTag.Substring(1))
Write-Host "Encoded tag: $EncodedPlayerTag" -ForegroundColor Gray

try {
    $player = Invoke-RestMethod -Uri "$baseUrl/player/$EncodedPlayerTag" -Method Get -TimeoutSec 10
    Write-Host "✅ SUCCESS! Player Found:" -ForegroundColor Green
    Write-Host "   Name: $($player.name)" -ForegroundColor White
    Write-Host "   Tag: $($player.tag)" -ForegroundColor White
    Write-Host "   Trophies: $($player.trophies)" -ForegroundColor White
    Write-Host "   Level: $($player.expLevel)" -ForegroundColor White
} catch {
    Write-Host "❌ FAILED to fetch player $TestPlayerTag" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
    
    # Check if it's a 404 specifically
    if ($_.Exception.Response.StatusCode -eq 404) {
        Write-Host "   This is a 404 error - player not found or API key issue" -ForegroundColor Yellow
    }
}
