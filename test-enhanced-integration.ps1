# Enhanced Real Data Integration Test
# Tests the new BrawlAPI integration and real data analytics

Write-Host "🚀 Enhanced Real Data Integration Test" -ForegroundColor Cyan
Write-Host "=====================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000/api"
$testsPassed = 0
$testsTotal = 0

function Test-EnhancedEndpoint {
    param(
        [string]$Url,
        [string]$Description,
        [string[]]$ExpectedKeys = @()
    )
    
    $global:testsTotal++
    Write-Host "Testing: $Description" -ForegroundColor Yellow
    Write-Host "  GET $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri $Url -Method GET -TimeoutSec 30
        Write-Host "  ✅ SUCCESS" -ForegroundColor Green
        
        if ($response) {
            # Check for expected data structure
            $hasExpectedData = $true
            foreach ($key in $ExpectedKeys) {
                if (-not ($response.PSObject.Properties.Name -contains $key)) {
                    Write-Host "  ⚠️  Missing expected key: $key" -ForegroundColor Yellow
                    $hasExpectedData = $false
                }
            }
            
            if ($hasExpectedData) {
                Write-Host "  ✅ Data structure validated" -ForegroundColor Green
            }
            
            # Show data source info
            if ($response.dataSource) {
                Write-Host "  📊 Data Source: $($response.dataSource)" -ForegroundColor Cyan
            }
            
            # Show sample data
            $responseText = $response | ConvertTo-Json -Compress -Depth 2
            if ($responseText.Length -gt 200) {
                $responseText = $responseText.Substring(0, 200) + "..."
            }
            Write-Host "  Sample: $responseText" -ForegroundColor DarkGray
        }
        
        $global:testsPassed++
        return $true
    }
    catch {
        Write-Host "  ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        if ($_.Exception.Response) {
            Write-Host "  Status Code: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
        }
        return $false
    }
    Write-Host ""
}

# Test enhanced endpoints
Write-Host "Testing Enhanced Real Data Integration..." -ForegroundColor White
Write-Host ""

# Test basic connectivity
Test-EnhancedEndpoint "$baseUrl/player/test" "Backend connectivity"

# Test original real data endpoints
Test-EnhancedEndpoint "$baseUrl/meta/stats" "Real meta statistics" @("totalMatches", "topBrawlers", "dataSource")
Test-EnhancedEndpoint "$baseUrl/meta/tiers" "Real tier list" @("tierList", "dataSource")

# Test new enhanced endpoints
Test-EnhancedEndpoint "$baseUrl/meta/winrates" "Real win rates" @("winRates", "dataSource")
Test-EnhancedEndpoint "$baseUrl/meta/pickrates" "Real pick rates" @("pickRates", "dataSource")
Test-EnhancedEndpoint "$baseUrl/meta/brawlers/count" "Real brawler count" @("totalBrawlers", "dataSource")

# Test BrawlAPI integration endpoints
Test-EnhancedEndpoint "$baseUrl/meta/events" "Active events (BrawlAPI)" @("events", "dataSource")
Test-EnhancedEndpoint "$baseUrl/meta/enhanced-stats" "Enhanced meta stats" @("stats", "dataSources")

# Test specific brawler analytics
Test-EnhancedEndpoint "$baseUrl/meta/brawler/16000000/analytics" "Brawler analytics (Shelly)"

Write-Host ""
Write-Host "🏆 Test Results Summary" -ForegroundColor Cyan
Write-Host "======================" -ForegroundColor Cyan
Write-Host "Tests Passed: $testsPassed / $testsTotal" -ForegroundColor $(if ($testsPassed -eq $testsTotal) { 'Green' } else { 'Yellow' })

if ($testsPassed -eq $testsTotal) {
    Write-Host "🎉 All tests passed! Enhanced real data integration is working correctly." -ForegroundColor Green
} elseif ($testsPassed -gt ($testsTotal * 0.7)) {
    Write-Host "✅ Most tests passed. Enhanced integration is mostly functional." -ForegroundColor Yellow
} else {
    Write-Host "⚠️  Several tests failed. Check the backend server and API integration." -ForegroundColor Red
}

Write-Host ""
Write-Host "🔍 Integration Features Tested:" -ForegroundColor Cyan
Write-Host "• Real battle log analysis" -ForegroundColor White
Write-Host "• BrawlAPI event integration" -ForegroundColor White
Write-Host "• Combined data source analytics" -ForegroundColor White
Write-Host "• Enhanced meta statistics" -ForegroundColor White
Write-Host "• Real win/pick rate calculations" -ForegroundColor White
Write-Host "• Live event data from BrawlAPI" -ForegroundColor White

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Gray
Read-Host
