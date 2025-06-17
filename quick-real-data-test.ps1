# Quick Real Data Integration Test
Write-Host "🚀 Quick Real Data Test" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5001/api"

function Test-Endpoint {
    param([string]$Url, [string]$Description)
    
    Write-Host "Testing: $Description" -ForegroundColor Yellow
    try {
        $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 15
        Write-Host "  ✅ SUCCESS" -ForegroundColor Green
        
        if ($response.dataSource) {
            Write-Host "  📊 Data Source: $($response.dataSource)" -ForegroundColor Cyan
        }
        
        return $true
    }
    catch {
        Write-Host "  ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

Write-Host "Testing Real Data Endpoints..." -ForegroundColor White
Write-Host ""

Test-Endpoint "$baseUrl/player/test" "Backend connectivity"
Test-Endpoint "$baseUrl/meta/stats" "Real meta statistics"
Test-Endpoint "$baseUrl/meta/tiers" "Real tier list"
Test-Endpoint "$baseUrl/meta/winrates" "Real win rates"
Test-Endpoint "$baseUrl/meta/events" "BrawlAPI events"

Write-Host ""
Write-Host "✅ Real data integration test complete!" -ForegroundColor Green
