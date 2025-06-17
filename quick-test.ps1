Write-Host "Testing Brawl Buddy API..." -ForegroundColor Cyan

# Test if backend is running
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/brawler" -UseBasicParsing -TimeoutSec 5
    Write-Host "✅ Backend is running on port 5000" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend not running. Start it with:" -ForegroundColor Red
    Write-Host "   cd BrawlBuddy.Api" -ForegroundColor Gray
    Write-Host "   dotnet run --urls=http://localhost:5000" -ForegroundColor Gray
    exit 1
}

# Test player API endpoint
$playerTag = "P9VQPV9YC"  # Without the # symbol
$apiUrl = "http://localhost:5000/api/player/$playerTag"

Write-Host "Testing player endpoint: $apiUrl" -ForegroundColor Yellow

try {
    $player = Invoke-RestMethod -Uri $apiUrl -Method Get -TimeoutSec 10
    Write-Host "✅ SUCCESS!" -ForegroundColor Green
    Write-Host "Player: $($player.name) - $($player.trophies) trophies" -ForegroundColor White
} catch {
    Write-Host "❌ Failed: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($_.Exception.Response) {
        $statusCode = [int]$_.Exception.Response.StatusCode
        Write-Host "Status Code: $statusCode" -ForegroundColor Red
        
        if ($statusCode -eq 404) {
            Write-Host "This could mean:" -ForegroundColor Yellow
            Write-Host "1. Player tag doesn't exist in Brawl Stars" -ForegroundColor Gray
            Write-Host "2. API key IP restriction issue" -ForegroundColor Gray
            Write-Host "3. Backend not using real API data" -ForegroundColor Gray
        }
    }
}
