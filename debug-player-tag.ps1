# Debug Player Tag Formatting
Write-Host "=== Player Tag Debug ===" -ForegroundColor Cyan

$TestTag = "#P9VQPV9YC"
Write-Host "Original tag: $TestTag" -ForegroundColor Yellow

# Test URL encoding (what the frontend would do)
$EncodedTag = [System.Web.HttpUtility]::UrlEncode($TestTag.Substring(1))
Write-Host "URL encoded (without #): $EncodedTag" -ForegroundColor Green

# Test what the API should receive
$ApiUrl = "http://localhost:5000/api/player/$EncodedTag"
Write-Host "Full API URL: $ApiUrl" -ForegroundColor Cyan

Write-Host ""
Write-Host "=== Testing API Call ===" -ForegroundColor Cyan

try {
    # First check if backend is running
    $healthCheck = Invoke-WebRequest -Uri "http://localhost:5000/api/brawler" -Method Get -TimeoutSec 3
    Write-Host "✅ Backend is running!" -ForegroundColor Green
    
    # Now test the specific player
    Write-Host "Testing player: $TestTag" -ForegroundColor Yellow
    $response = Invoke-RestMethod -Uri $ApiUrl -Method Get -TimeoutSec 10
    
    Write-Host "✅ SUCCESS! Player found:" -ForegroundColor Green
    Write-Host "  Name: $($response.name)" -ForegroundColor White
    Write-Host "  Tag: $($response.tag)" -ForegroundColor White
    Write-Host "  Trophies: $($response.trophies)" -ForegroundColor White
    Write-Host "  Level: $($response.expLevel)" -ForegroundColor White
    
} catch {
    $StatusCode = $_.Exception.Response.StatusCode.value__
    Write-Host "❌ FAILED:" -ForegroundColor Red
    Write-Host "  Status Code: $StatusCode" -ForegroundColor Red
    Write-Host "  Error: $($_.Exception.Message)" -ForegroundColor Red
    
    if ($StatusCode -eq 404) {
        Write-Host ""
        Write-Host "🔍 This is likely one of these issues:" -ForegroundColor Yellow
        Write-Host "  1. Player tag doesn't exist" -ForegroundColor Gray
        Write-Host "  2. API key IP restriction" -ForegroundColor Gray
        Write-Host "  3. Tag formatting issue" -ForegroundColor Gray
        Write-Host "  4. Backend not using real API" -ForegroundColor Gray
    }
}

Write-Host ""
Write-Host "=== Next Steps ===" -ForegroundColor Cyan
Write-Host "1. Make sure backend is running: cd BrawlBuddy.Api && dotnet run" -ForegroundColor Gray
Write-Host "2. Try the search again in your app" -ForegroundColor Gray
Write-Host "3. If still fails, try a different player tag" -ForegroundColor Gray
