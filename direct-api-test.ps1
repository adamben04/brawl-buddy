# Direct API Test for your player tag
$playerTag = "P9VQPV9YC"
$baseUrl = "http://localhost:5000"

Write-Host "Testing Direct API Call for Player: #$playerTag" -ForegroundColor Cyan
Write-Host "=" * 50

# Step 1: Check if backend is running
Write-Host "1. Checking if backend is running..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "$baseUrl/api/brawler" -UseBasicParsing -TimeoutSec 5
    Write-Host "   ✅ Backend is running (Status: $($response.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Backend not running!" -ForegroundColor Red
    Write-Host "   Please start: cd BrawlBuddy.Api && dotnet run --urls=http://localhost:5000" -ForegroundColor Gray
    exit 1
}

# Step 2: Test player endpoint directly
Write-Host "`n2. Testing player endpoint..." -ForegroundColor Yellow
$playerUrl = "$baseUrl/api/player/$playerTag"
Write-Host "   URL: $playerUrl" -ForegroundColor Gray

try {
    # Use Invoke-WebRequest first to get detailed response info
    $webResponse = Invoke-WebRequest -Uri $playerUrl -UseBasicParsing -TimeoutSec 15
    Write-Host "   ✅ SUCCESS! Status Code: $($webResponse.StatusCode)" -ForegroundColor Green
    
    # Now get the actual JSON data
    $player = Invoke-RestMethod -Uri $playerUrl -Method Get -TimeoutSec 15
    Write-Host "   Player Name: $($player.name)" -ForegroundColor White
    Write-Host "   Player Tag: $($player.tag)" -ForegroundColor White
    Write-Host "   Trophies: $($player.trophies)" -ForegroundColor White
    Write-Host "   Level: $($player.expLevel)" -ForegroundColor White
    
} catch {
    Write-Host "   ❌ FAILED!" -ForegroundColor Red
    $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { "Unknown" }
    Write-Host "   Status Code: $statusCode" -ForegroundColor Red
    Write-Host "   Error Message: $($_.Exception.Message)" -ForegroundColor Red
    
    # Try to get response content for more details
    if ($_.Exception.Response) {
        try {
            $errorStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorStream)
            $errorContent = $reader.ReadToEnd()
            Write-Host "   Response Content: $errorContent" -ForegroundColor DarkGray
        } catch {
            Write-Host "   Could not read error response" -ForegroundColor DarkGray
        }
    }
}

Write-Host "`n3. Testing with # prefix..." -ForegroundColor Yellow
$playerUrlWithHash = "$baseUrl/api/player/%23$playerTag"
Write-Host "   URL: $playerUrlWithHash" -ForegroundColor Gray

try {
    $player2 = Invoke-RestMethod -Uri $playerUrlWithHash -Method Get -TimeoutSec 15
    Write-Host "   ✅ SUCCESS with # prefix!" -ForegroundColor Green
    Write-Host "   Player Name: $($player2.name)" -ForegroundColor White
} catch {
    $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { "Unknown" }
    Write-Host "   ❌ Also failed with # prefix (Status: $statusCode)" -ForegroundColor Red
}

Write-Host "`n" + "=" * 50
Write-Host "If both tests failed with 404, the issue is likely:" -ForegroundColor Yellow
Write-Host "• Your current IP doesn't match the whitelisted IP (174.21.83.202)" -ForegroundColor Gray
Write-Host "• API key has expired or is invalid" -ForegroundColor Gray
Write-Host "• Backend is still using mock data somehow" -ForegroundColor Gray
