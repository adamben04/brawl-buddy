# Test Real Brawler Data Integration
Write-Host "🧪 Testing Real Brawler Data Integration" -ForegroundColor Cyan
Write-Host "=======================================" -ForegroundColor Cyan

$baseUrl = "http://localhost:5001/api"

function Test-EndpointDetailed {
    param(
        [string]$Url,
        [string]$Description
    )
    
    Write-Host "`n🔍 Testing: $Description" -ForegroundColor Yellow
    Write-Host "   URL: $Url" -ForegroundColor Gray
    
    try {
        $response = Invoke-RestMethod -Uri $Url -Method Get -TimeoutSec 20
        Write-Host "   ✅ SUCCESS" -ForegroundColor Green
        
        # Detailed analysis of response
        if ($response.topBrawlers) {
            Write-Host "   📊 Top Brawlers Found:" -ForegroundColor Cyan
            foreach ($brawler in $response.topBrawlers[0..2]) {
                Write-Host "      - $($brawler.name): WR $($brawler.winRate)%, PR $($brawler.pickRate)%" -ForegroundColor White
                if ($brawler.dataSource) {
                    Write-Host "        Source: $($brawler.dataSource)" -ForegroundColor Gray
                }
            }
        }
        
        if ($response.totalBrawlers) {
            Write-Host "   🎮 Total Brawlers: $($response.totalBrawlers)" -ForegroundColor Cyan
        }
        
        if ($response.dataSource) {
            Write-Host "   📡 Data Source: $($response.dataSource)" -ForegroundColor Magenta
        }
        
        if ($response.events) {
            Write-Host "   🗓️ Active Events: $($response.events.Count)" -ForegroundColor Cyan
            foreach ($event in $response.events[0..1]) {
                Write-Host "      - $($event.eventName): $($event.mapName)" -ForegroundColor White
            }
        }
        
        return $true
    }
    catch {
        Write-Host "   ❌ FAILED: $($_.Exception.Message)" -ForegroundColor Red
        return $false
    }
}

# Test the key endpoints that the home page uses
Write-Host "Testing Enhanced Meta Stats (Main Home Page Data)..."
$enhancedSuccess = Test-EndpointDetailed "$baseUrl/meta/enhanced-stats" "Enhanced Meta Statistics"

Write-Host "`nTesting Brawler Count..."
$countSuccess = Test-EndpointDetailed "$baseUrl/meta/brawlers/count" "Brawler Count"

Write-Host "`nTesting Active Events..."
$eventsSuccess = Test-EndpointDetailed "$baseUrl/meta/events" "Active Events"

Write-Host "`nTesting Basic Meta Stats (Fallback)..."
$basicSuccess = Test-EndpointDetailed "$baseUrl/meta/stats" "Basic Meta Statistics"

# Summary
Write-Host "`n" + "="*50 -ForegroundColor Cyan
Write-Host "📊 HOME PAGE DATA READINESS" -ForegroundColor Cyan
Write-Host "="*50 -ForegroundColor Cyan

$totalTests = 4
$passedTests = @($enhancedSuccess, $countSuccess, $eventsSuccess, $basicSuccess) | Where-Object { $_ -eq $true } | Measure-Object | Select-Object -ExpandProperty Count

Write-Host "✅ Passed Tests: $passedTests/$totalTests" -ForegroundColor $(if ($passedTests -eq $totalTests) { "Green" } else { "Yellow" })

if ($enhancedSuccess) {
    Write-Host "🎉 Enhanced stats working - home page will show real brawler names and stats!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Enhanced stats failed - home page may show limited data" -ForegroundColor Yellow
}

if ($eventsSuccess) {
    Write-Host "📅 Current events available - home page will show live events!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Events data unavailable - home page won't show current events" -ForegroundColor Yellow
}

if ($countSuccess) {
    Write-Host "🔢 Real brawler count available!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Brawler count may be inaccurate" -ForegroundColor Yellow
}

Write-Host "`n🎯 Next Steps:" -ForegroundColor White
if ($passedTests -eq $totalTests) {
    Write-Host "   ✅ All endpoints ready - frontend home page will display real data" -ForegroundColor Green
    Write-Host "   ✅ BrawlAPI integration successful" -ForegroundColor Green
    Write-Host "   🚀 Home page ready for real data display!" -ForegroundColor Green
} else {
    Write-Host "   🔧 Some endpoints need attention" -ForegroundColor Yellow
    Write-Host "   📝 Check API logs for detailed error information" -ForegroundColor Yellow
}
