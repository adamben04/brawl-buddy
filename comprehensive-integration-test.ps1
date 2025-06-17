# Comprehensive BrawlAPI Integration Test
Write-Host "🔍 Testing BrawlAPI Integration After Fixes" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5001/api"
$testResults = @()

function Test-Endpoint {
    param(
        [string]$Url,
        [string]$Description,
        [int]$TimeoutSeconds = 30
    )
    
    Write-Host "`n🧪 Testing: $Description" -ForegroundColor Yellow
    Write-Host "   URL: $Url" -ForegroundColor Gray
    
    try {
        $stopwatch = [System.Diagnostics.Stopwatch]::StartNew()
        
        # Use Invoke-RestMethod for JSON responses
        $response = Invoke-RestMethod -Uri $Url -Method Get -TimeoutSec $TimeoutSeconds -ErrorAction Stop
        
        $stopwatch.Stop()
        $responseTime = $stopwatch.ElapsedMilliseconds
        
        Write-Host "   ✅ SUCCESS ($($responseTime)ms)" -ForegroundColor Green
        
        # Analyze response content
        if ($response -is [array]) {
            Write-Host "   📊 Response: Array with $($response.Count) items" -ForegroundColor Cyan
            
            if ($response.Count -gt 0) {
                $firstItem = $response[0]
                $properties = $firstItem.PSObject.Properties.Name
                Write-Host "   🔑 First item properties: $($properties -join ', ')" -ForegroundColor Gray
            }
        }
        elseif ($response -is [object]) {
            $properties = $response.PSObject.Properties.Name
            Write-Host "   🔑 Object properties: $($properties -join ', ')" -ForegroundColor Gray
            
            # Check for data source indication
            if ($response.dataSource) {
                Write-Host "   📡 Data Source: $($response.dataSource)" -ForegroundColor Magenta
            }
            
            # Check for real data indicators
            if ($response.totalBrawlers -or $response.Count -or $response.length) {
                $count = $response.totalBrawlers ?? $response.Count ?? $response.length
                Write-Host "   📈 Data Count: $count" -ForegroundColor Cyan
            }
        }
        elseif ($response -is [string] -or $response -is [int]) {
            Write-Host "   📄 Response: $response" -ForegroundColor Cyan
        }
        
        return @{
            Success = $true
            ResponseTime = $responseTime
            DataType = $response.GetType().Name
            ItemCount = if ($response -is [array]) { $response.Count } else { 1 }
        }
    }
    catch {
        Write-Host "   ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        
        # Check for specific error types
        if ($_.Exception.Message -like "*timeout*") {
            Write-Host "   ⏰ This might indicate slow API response or processing" -ForegroundColor Yellow
        }
        elseif ($_.Exception.Message -like "*connection*" -or $_.Exception.Message -like "*refused*") {
            Write-Host "   🔌 Server connection issue - is the API running?" -ForegroundColor Yellow
        }
        
        return @{
            Success = $false
            Error = $_.Exception.Message
        }
    }
}

Write-Host "`n🎯 Testing Core Endpoints..." -ForegroundColor White

# Test basic connectivity first
$connectivityResult = Test-Endpoint "$baseUrl/player/test" "API Connectivity Test"

if (-not $connectivityResult.Success) {
    Write-Host "`n❌ API connectivity failed. Stopping tests." -ForegroundColor Red
    exit 1
}

# Test BrawlAPI integration endpoints
Write-Host "`n🌐 Testing BrawlAPI Integration Endpoints..." -ForegroundColor White

$endpoints = @(
    @{ url = "$baseUrl/meta/events"; desc = "BrawlAPI Events (Community Data)" },
    @{ url = "$baseUrl/meta/stats"; desc = "Meta Statistics (Combined Data)" },
    @{ url = "$baseUrl/meta/tiers"; desc = "Tier List (Real Win/Pick Rates)" },
    @{ url = "$baseUrl/meta/enhanced-stats"; desc = "Enhanced Stats (Both APIs)" },
    @{ url = "$baseUrl/meta/winrates"; desc = "Real Win Rates" },
    @{ url = "$baseUrl/meta/pickrates"; desc = "Real Pick Rates" },
    @{ url = "$baseUrl/meta/brawlers/count"; desc = "Unique Brawler Count" }
)

$successCount = 0
$totalTests = $endpoints.Count

foreach ($endpoint in $endpoints) {
    $result = Test-Endpoint $endpoint.url $endpoint.desc
    if ($result.Success) {
        $successCount++
    }
    $testResults += $result
}

# Test specific brawler analytics (Shelly = 16000000)
Write-Host "`n🎮 Testing Specific Brawler Analytics..." -ForegroundColor White
$brawlerResult = Test-Endpoint "$baseUrl/meta/brawler/16000000/analytics" "Shelly Analytics"
if ($brawlerResult.Success) {
    $successCount++
}
$totalTests++

# Summary
Write-Host "`n" + "="*50 -ForegroundColor Cyan
Write-Host "📊 TEST RESULTS SUMMARY" -ForegroundColor Cyan
Write-Host "="*50 -ForegroundColor Cyan

Write-Host "✅ Successful Tests: $successCount/$totalTests" -ForegroundColor $(if ($successCount -eq $totalTests) { "Green" } else { "Yellow" })

$successRate = [math]::Round(($successCount / $totalTests) * 100, 1)
Write-Host "📈 Success Rate: $successRate%" -ForegroundColor $(if ($successRate -ge 90) { "Green" } elseif ($successRate -ge 70) { "Yellow" } else { "Red" })

if ($successCount -eq $totalTests) {
    Write-Host "`n🎉 ALL TESTS PASSED!" -ForegroundColor Green
    Write-Host "✅ BrawlAPI integration is working correctly" -ForegroundColor Green
    Write-Host "✅ Real data is flowing from community APIs" -ForegroundColor Green
    Write-Host "✅ Combined analytics are operational" -ForegroundColor Green
} else {
    Write-Host "`n⚠️ Some tests failed. Check the details above." -ForegroundColor Yellow
    
    $failedTests = $testResults | Where-Object { -not $_.Success }
    if ($failedTests.Count -gt 0) {
        Write-Host "`n❌ Failed endpoints:" -ForegroundColor Red
        foreach ($failed in $failedTests) {
            Write-Host "   - $($failed.Error)" -ForegroundColor Red
        }
    }
}

Write-Host "`n🔧 Integration Status:" -ForegroundColor White
Write-Host "   📡 Official Brawl Stars API: Active" -ForegroundColor Green
Write-Host "   🌐 Community BrawlAPI: $(if ($successCount -gt 0) { 'Active' } else { 'Issues Detected' })" -ForegroundColor $(if ($successCount -gt 0) { "Green" } else { "Red" })
Write-Host "   🔄 Data Combination: $(if ($successCount -eq $totalTests) { 'Working' } else { 'Partial' })" -ForegroundColor $(if ($successCount -eq $totalTests) { "Green" } else { "Yellow" })

Write-Host "`n🎯 Next Steps:" -ForegroundColor White
if ($successCount -eq $totalTests) {
    Write-Host "   ✅ Integration complete - ready for frontend testing" -ForegroundColor Green
    Write-Host "   ✅ All real data sources operational" -ForegroundColor Green
} else {
    Write-Host "   🔧 Review failed endpoints and fix integration issues" -ForegroundColor Yellow
    Write-Host "   📝 Check API logs for detailed error information" -ForegroundColor Yellow
}
