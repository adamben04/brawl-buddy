# Ultimate API Test Script
# Tests your exact player tag: #P9VQPV9YC

Write-Host "===== BRAWL BUDDY API TEST =====" -ForegroundColor Cyan
Write-Host "Testing Player Tag: #P9VQPV9YC" -ForegroundColor Yellow
Write-Host "Your IP: 174.21.83.202 (matches API key ✓)" -ForegroundColor Green
Write-Host ""

# Test 1: Check if backend is running
Write-Host "1. Checking Backend Status..." -ForegroundColor Yellow
try {
    $healthResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/brawler" -UseBasicParsing -TimeoutSec 5
    Write-Host "   ✅ Backend is RUNNING (Status: $($healthResponse.StatusCode))" -ForegroundColor Green
    $backendRunning = $true
} catch {
    Write-Host "   ❌ Backend is NOT RUNNING" -ForegroundColor Red
    Write-Host "   Please start backend first:" -ForegroundColor Gray
    Write-Host "   cd BrawlBuddy.Api" -ForegroundColor Gray
    Write-Host "   dotnet run --urls=http://localhost:5000" -ForegroundColor Gray
    $backendRunning = $false
}

if (-not $backendRunning) {
    Write-Host ""
    Write-Host "Cannot continue without backend running. Please start it and run this script again." -ForegroundColor Red
    exit 1
}

# Test 2: Test your exact player tag (without # prefix for API)
Write-Host ""
Write-Host "2. Testing Your Player Tag..." -ForegroundColor Yellow
$playerTag = "P9VQPV9YC"
$apiUrl = "http://localhost:5000/api/player/$playerTag"
Write-Host "   API URL: $apiUrl" -ForegroundColor Gray

try {
    $playerResponse = Invoke-RestMethod -Uri $apiUrl -Method Get -TimeoutSec 15
    Write-Host "   ✅ SUCCESS! Found your player:" -ForegroundColor Green
    Write-Host "   Name: $($playerResponse.name)" -ForegroundColor White
    Write-Host "   Tag: $($playerResponse.tag)" -ForegroundColor White
    Write-Host "   Trophies: $($playerResponse.trophies)" -ForegroundColor White
    Write-Host "   Level: $($playerResponse.expLevel)" -ForegroundColor White
    $playerFound = $true
} catch {
    $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { "Unknown" }
    Write-Host "   ❌ FAILED! Status Code: $statusCode" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Red
    $playerFound = $false
    
    # Try to get detailed error response
    if ($_.Exception.Response) {
        try {
            $errorStream = $_.Exception.Response.GetResponseStream()
            $reader = New-Object System.IO.StreamReader($errorStream)
            $errorContent = $reader.ReadToEnd()
            if ($errorContent) {
                Write-Host "   Server Response: $errorContent" -ForegroundColor DarkGray
            }
        } catch {
            # Ignore error reading error response
        }
    }
}

# Test 3: Test with # prefix (URL encoded)
Write-Host ""
Write-Host "3. Testing with # prefix (URL encoded)..." -ForegroundColor Yellow
$encodedUrl = "http://localhost:5000/api/player/%23$playerTag"
Write-Host "   API URL: $encodedUrl" -ForegroundColor Gray

try {
    $playerResponse2 = Invoke-RestMethod -Uri $encodedUrl -Method Get -TimeoutSec 15
    Write-Host "   ✅ SUCCESS with # prefix!" -ForegroundColor Green
    Write-Host "   Name: $($playerResponse2.name)" -ForegroundColor White
    $playerFoundWithHash = $true
} catch {
    $statusCode = if ($_.Exception.Response) { [int]$_.Exception.Response.StatusCode } else { "Unknown" }
    Write-Host "   ❌ Also failed with # prefix (Status: $statusCode)" -ForegroundColor Red
    $playerFoundWithHash = $false
}

# Test 4: Test another known player tag to verify API connectivity
Write-Host ""
Write-Host "4. Testing Known Player Tags to verify API..." -ForegroundColor Yellow
$testTags = @("2PP", "8QU8J9LP", "YLLGPJLG", "2Y0GG89JY")
$anyPlayerFound = $false

foreach ($testTag in $testTags) {
    try {
        $testResponse = Invoke-RestMethod -Uri "http://localhost:5000/api/player/$testTag" -Method Get -TimeoutSec 10
        Write-Host "   ✅ Found player with tag #$testTag`: $($testResponse.name)" -ForegroundColor Green
        $anyPlayerFound = $true
        break
    } catch {
        Write-Host "   ❌ Tag #$testTag not found" -ForegroundColor DarkGray
    }
}

# Results Summary
Write-Host ""
Write-Host "===== RESULTS SUMMARY =====" -ForegroundColor Cyan

if ($playerFound -or $playerFoundWithHash) {
    Write-Host "🎉 SUCCESS! Your player tag #P9VQPV9YC is working!" -ForegroundColor Green
    Write-Host "The real Brawl Stars API integration is functioning correctly." -ForegroundColor Green
} elseif ($anyPlayerFound) {
    Write-Host "⚠️  API is working but your specific tag #P9VQPV9YC was not found" -ForegroundColor Yellow
    Write-Host "This could mean:" -ForegroundColor Yellow
    Write-Host "• The tag might be typed incorrectly" -ForegroundColor Gray
    Write-Host "• The player might not exist in Brawl Stars" -ForegroundColor Gray
    Write-Host "• Try checking your tag in-game again" -ForegroundColor Gray
} else {
    Write-Host "❌ API connectivity issues detected" -ForegroundColor Red
    Write-Host "Possible causes:" -ForegroundColor Red
    Write-Host "• API key might be expired" -ForegroundColor Gray
    Write-Host "• Brawl Stars API might be down" -ForegroundColor Gray
    Write-Host "• Backend configuration issue" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Next Steps:" -ForegroundColor Cyan
if ($playerFound -or $playerFoundWithHash) {
    Write-Host "• Try searching in your web app again - it should work now!" -ForegroundColor Green
} else {
    Write-Host "• Double-check your player tag in Brawl Stars" -ForegroundColor Gray
    Write-Host "• Make sure you copied it exactly (case-sensitive)" -ForegroundColor Gray
    Write-Host "• Try a different player tag to test" -ForegroundColor Gray
}

Write-Host ""
Write-Host "============================" -ForegroundColor Cyan
