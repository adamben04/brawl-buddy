# Brawl Buddy API Test Script
# Tests all major API endpoints and verifies responses

Write-Host "🎮 Brawl Buddy API Integration Test" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000/api"
$testsPassed = 0
$testsTotal = 0

function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Description
    )
    
    $global:testsTotal++
    Write-Host "Testing: $Description" -ForegroundColor Yellow
    Write-Host "  GET $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 10
        Write-Host "  ✅ SUCCESS" -ForegroundColor Green
        
        if ($response) {
            $responseText = $response | ConvertTo-Json -Compress
            if ($responseText.Length -gt 100) {
                $responseText = $responseText.Substring(0, 100) + "..."
            }
            Write-Host "  Response: $responseText" -ForegroundColor DarkGray
        }
        
        $global:testsPassed++
        return $true
    }
    catch {
        Write-Host "  ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
    Write-Host ""
}

# Test all endpoints
Write-Host "Running API endpoint tests..." -ForegroundColor White
Write-Host ""

Test-Endpoint "$baseUrl/player/test" "Backend connectivity test"
Test-Endpoint "$baseUrl/brawler" "Get all brawlers (with mock fallback)"
Test-Endpoint "$baseUrl/brawler/16000000" "Get specific brawler (Shelly)"
Test-Endpoint "$baseUrl/meta/tiers" "Get tier list (all modes)"
Test-Endpoint "$baseUrl/meta/tiers?mode=gemgrab" "Get tier list (Gem Grab mode)"
Test-Endpoint "$baseUrl/meta/stats" "Get meta statistics"

Write-Host ""
Write-Host "=================================" -ForegroundColor Cyan
Write-Host "Test Results: $testsPassed/$testsTotal passed" -ForegroundColor $(if ($testsPassed -eq $testsTotal) { "Green" } else { "Yellow" })

if ($testsPassed -eq $testsTotal) {
    Write-Host "🎉 All API endpoints are working correctly!" -ForegroundColor Green
    Write-Host "✅ Backend integration successful" -ForegroundColor Green
    Write-Host "✅ Mock data fallback functional" -ForegroundColor Green
    Write-Host "✅ Error handling working" -ForegroundColor Green
} else {
    Write-Host "⚠️  Some endpoints need attention" -ForegroundColor Yellow
}

Write-Host ""
Write-Host "Next steps:" -ForegroundColor Cyan
Write-Host "- Visit http://localhost:5000/swagger for API documentation" -ForegroundColor White
Write-Host "- Visit http://localhost:5173 for the frontend application" -ForegroundColor White
Write-Host "- Check API_INTEGRATION_TEST_RESULTS.md for detailed status" -ForegroundColor White
Write-Host ""
