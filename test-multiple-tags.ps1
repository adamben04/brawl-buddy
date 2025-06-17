# Test known player tags to verify API is working
Write-Host "Testing Brawl Stars API with various player tags..." -ForegroundColor Cyan

$baseUrl = "http://localhost:5000/api"

# Test backend health first
try {
    $health = Invoke-RestMethod -Uri "$baseUrl/brawler" -Method Get -TimeoutSec 5
    Write-Host "✅ Backend is responding" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend not running or not responding" -ForegroundColor Red
    Write-Host "Please start backend: cd BrawlBuddy.Api && dotnet run" -ForegroundColor Gray
    exit 1
}

# Test different player tags (some might work, some might not)
$testTags = @(
    "P9VQPV9YC",     # Your original tag
    "2PP0L2U2V",     # The first tag you tried  
    "8QU8J9LP",      # A potentially common format
    "YLLGPJLG",      # Another common format
    "2Y0GG89JY"      # Another test tag
)

foreach ($tag in $testTags) {
    Write-Host "`nTesting tag: #$tag" -ForegroundColor Yellow
    
    try {
        $player = Invoke-RestMethod -Uri "$baseUrl/player/$tag" -Method Get -TimeoutSec 10
        Write-Host "✅ SUCCESS: $($player.name) - $($player.trophies) trophies" -ForegroundColor Green
        break  # If we find one that works, we know the API is functioning
    } catch {
        $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { "Unknown" }
        Write-Host "❌ Failed with status: $statusCode" -ForegroundColor Red
    }
}

Write-Host "`n=== Diagnosis ===" -ForegroundColor Cyan
Write-Host "If ALL tags failed with 404:" -ForegroundColor Yellow
Write-Host "  • API key might be invalid or expired" -ForegroundColor Gray
Write-Host "  • IP address not whitelisted" -ForegroundColor Gray
Write-Host "  • Backend might still be using mock data" -ForegroundColor Gray
Write-Host "`nIf SOME tags work:" -ForegroundColor Yellow
Write-Host "  • The specific player tag doesn't exist" -ForegroundColor Gray
Write-Host "  • Try finding your actual player tag in-game" -ForegroundColor Gray
