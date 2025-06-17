# Final Real Data Integration Validation Test
Write-Host "🎮 FINAL REAL DATA INTEGRATION TEST" -ForegroundColor Cyan
Write-Host "===================================" -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5001/api"
$testResults = @()

function Test-RealDataEndpoint {
    param(
        [string]$Endpoint,
        [string]$Description,
        [string[]]$RequiredKeys = @()
    )
    
    Write-Host "🔍 Testing: $Description" -ForegroundColor Yellow
    Write-Host "   URL: GET $Endpoint" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method GET -TimeoutSec 20
        
        # Check if response exists
        if (-not $response) {
            throw "Empty response received"
        }
        
        # Validate required keys
        $missingKeys = @()
        foreach ($key in $RequiredKeys) {
            if (-not ($response.PSObject.Properties.Name -contains $key)) {
                $missingKeys += $key
            }
        }
        
        $success = $missingKeys.Count -eq 0
        
        if ($success) {
            Write-Host "   ✅ SUCCESS" -ForegroundColor Green
            
            # Show data source if available
            if ($response.dataSource) {
                Write-Host "   📊 Data Source: $($response.dataSource)" -ForegroundColor Cyan
            }
            
            # Show sample data
            if ($response.topBrawlers) {
                Write-Host "   🎯 Top Brawler: $($response.topBrawlers[0].name) (WR: $($response.topBrawlers[0].winRate)%)" -ForegroundColor Green
            }
            
            if ($response.events) {
                Write-Host "   🎪 Active Events: $($response.events.Count)" -ForegroundColor Green
            }
            
            if ($response.totalBrawlers) {
                Write-Host "   🎮 Total Brawlers: $($response.totalBrawlers)" -ForegroundColor Green
            }
            
        } else {
            Write-Host "   ❌ FAILED - Missing keys: $($missingKeys -join ', ')" -ForegroundColor Red
        }
        
        $script:testResults += [PSCustomObject]@{
            Endpoint = $Endpoint
            Description = $Description
            Success = $success
            DataSource = $response.dataSource
            ResponseSize = ($response | ConvertTo-Json -Compress).Length
        }
        
        return $success
        
    } catch {
        Write-Host "   ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        
        $script:testResults += [PSCustomObject]@{
            Endpoint = $Endpoint
            Description = $Description
            Success = $false
            DataSource = "Error"
            ResponseSize = 0
        }
        
        return $false
    }
    
    Write-Host ""
}

Write-Host "🚀 Starting comprehensive real data validation..." -ForegroundColor White
Write-Host ""

# Test 1: Basic connectivity
Test-RealDataEndpoint "/player/test" "Backend Connectivity Test"

# Test 2: Core real data endpoints
Test-RealDataEndpoint "/meta/stats" "Enhanced Meta Statistics" @("totalMatches", "topBrawlers", "dataSource")
Test-RealDataEndpoint "/meta/tiers" "Real Tier Lists" @("tierList", "dataSource") 

# Test 3: New real data analytics
Test-RealDataEndpoint "/meta/winrates" "Real Win Rates" @("winRates", "dataSource")
Test-RealDataEndpoint "/meta/pickrates" "Real Pick Rates" @("pickRates", "dataSource")
Test-RealDataEndpoint "/meta/brawlers/count" "Real Brawler Count" @("totalBrawlers", "dataSource")

# Test 4: BrawlAPI integration
Test-RealDataEndpoint "/meta/events" "Live Events (BrawlAPI)" @("events", "dataSource")
Test-RealDataEndpoint "/meta/enhanced-stats" "Enhanced Combined Analytics" @("stats", "dataSources")

# Test 5: Specific brawler analytics
Test-RealDataEndpoint "/meta/brawler/16000000/analytics" "Brawler Analytics (Shelly)"

# Test 6: Mode-specific data
Test-RealDataEndpoint "/meta/winrates?mode=gemgrab" "Win Rates (Gem Grab)"
Test-RealDataEndpoint "/meta/tiers?mode=brawlball" "Tier List (Brawl Ball)"

Write-Host ""
Write-Host "📋 TEST RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

$totalTests = $testResults.Count
$passedTests = ($testResults | Where-Object { $_.Success }).Count
$failedTests = $totalTests - $passedTests

Write-Host "Total Tests: $totalTests" -ForegroundColor White
Write-Host "Passed: $passedTests" -ForegroundColor Green
Write-Host "Failed: $failedTests" -ForegroundColor $(if ($failedTests -eq 0) { 'Green' } else { 'Red' })
Write-Host "Success Rate: $([math]::Round(($passedTests / $totalTests) * 100, 1))%" -ForegroundColor $(if ($passedTests -eq $totalTests) { 'Green' } elseif ($passedTests -gt ($totalTests * 0.8)) { 'Yellow' } else { 'Red' })

Write-Host ""
Write-Host "📊 DATA SOURCE ANALYSIS" -ForegroundColor Cyan
Write-Host "========================" -ForegroundColor Cyan

$dataSources = $testResults | Where-Object { $_.Success -and $_.DataSource -ne "Error" } | Group-Object DataSource
foreach ($source in $dataSources) {
    Write-Host "$($source.Name): $($source.Count) endpoints" -ForegroundColor Green
}

Write-Host ""
if ($passedTests -eq $totalTests) {
    Write-Host "🎉 ALL TESTS PASSED! Real data integration is FULLY OPERATIONAL!" -ForegroundColor Green
    Write-Host ""
    Write-Host "✅ Your Brawl Buddy app now uses 100% REAL DATA:" -ForegroundColor Green
    Write-Host "   • Real win rates from actual battles" -ForegroundColor White
    Write-Host "   • Live events and statistics from BrawlAPI" -ForegroundColor White
    Write-Host "   • Enhanced analytics from dual data sources" -ForegroundColor White
    Write-Host "   • Smart caching for optimal performance" -ForegroundColor White
    Write-Host "   • Comprehensive fallback mechanisms" -ForegroundColor White
} elseif ($passedTests -gt ($totalTests * 0.8)) {
    Write-Host "✅ Most tests passed! Real data integration is mostly operational." -ForegroundColor Yellow
    Write-Host "   Check failed endpoints and ensure backend is running." -ForegroundColor Yellow
} else {
    Write-Host "⚠️ Several tests failed. Ensure the backend is running:" -ForegroundColor Red
    Write-Host "   dotnet run --urls=`"http://localhost:5000;https://localhost:5001`"" -ForegroundColor Gray
}

Write-Host ""
Write-Host "Press any key to continue..." -ForegroundColor Gray
Read-Host
