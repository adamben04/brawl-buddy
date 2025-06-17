# Simple API Test with File Output
$logFile = "api-test-log.txt"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

"=== API Test Results - $timestamp ===" | Out-File $logFile

try {
    # Test BrawlAPI events endpoint
    Write-Host "Testing BrawlAPI events..." -ForegroundColor Yellow
    $events = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/events" -Method Get -TimeoutSec 15
    
    if ($events -and $events.Count -gt 0) {
        "✅ BrawlAPI Events: SUCCESS - $($events.Count) events found" | Out-File $logFile -Append
        
        # Check first event structure
        $firstEvent = $events[0]
        $eventProps = $firstEvent.PSObject.Properties.Name
        "   First event properties: $($eventProps -join ', ')" | Out-File $logFile -Append
        
        if ($firstEvent.map -and $firstEvent.map.stats) {
            "   Map stats available: $($firstEvent.map.stats.Count) brawler stats" | Out-File $logFile -Append
        }
    } else {
        "❌ BrawlAPI Events: No events returned" | Out-File $logFile -Append
    }
} catch {
    "❌ BrawlAPI Events: ERROR - $($_.Exception.Message)" | Out-File $logFile -Append
}

try {
    # Test enhanced stats
    Write-Host "Testing enhanced stats..." -ForegroundColor Yellow
    $enhancedStats = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/enhanced-stats" -Method Get -TimeoutSec 15
    
    if ($enhancedStats) {
        $statsProps = $enhancedStats.PSObject.Properties.Name
        "✅ Enhanced Stats: SUCCESS" | Out-File $logFile -Append
        "   Properties: $($statsProps -join ', ')" | Out-File $logFile -Append
        
        if ($enhancedStats.dataSource) {
            "   Data Source: $($enhancedStats.dataSource)" | Out-File $logFile -Append
        }
    } else {
        "❌ Enhanced Stats: No data returned" | Out-File $logFile -Append
    }
} catch {
    "❌ Enhanced Stats: ERROR - $($_.Exception.Message)" | Out-File $logFile -Append
}

try {
    # Test brawler count
    Write-Host "Testing brawler count..." -ForegroundColor Yellow
    $count = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/brawlers/count" -Method Get -TimeoutSec 10
    "✅ Brawler Count: $count" | Out-File $logFile -Append
} catch {
    "❌ Brawler Count: ERROR - $($_.Exception.Message)" | Out-File $logFile -Append
}

"=== Test Complete ===" | Out-File $logFile -Append

Write-Host "Test complete. Check api-test-log.txt for results." -ForegroundColor Green
