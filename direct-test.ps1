# Direct API Test
Write-Host "Testing API on port 5001..." -ForegroundColor Yellow

try {
    # Test basic connectivity
    $healthCheck = Invoke-WebRequest -Uri "http://localhost:5001" -Method Get -TimeoutSec 10
    Write-Host "✅ Server responding (Status: $($healthCheck.StatusCode))" -ForegroundColor Green
    
    # Test meta stats endpoint
    $metaStats = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/stats" -Method Get -TimeoutSec 15
    Write-Host "✅ Meta stats endpoint working" -ForegroundColor Green
    
    if ($metaStats.dataSource) {
        Write-Host "📊 Data Source: $($metaStats.dataSource)" -ForegroundColor Cyan
    }
    
    # Test brawler count endpoint
    $brawlerCount = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/brawlers/count" -Method Get -TimeoutSec 10
    Write-Host "✅ Brawler count: $brawlerCount" -ForegroundColor Green
    
    Write-Host "🎉 All tests passed!" -ForegroundColor Green
    
} catch {
    Write-Host "❌ Error: $($_.Exception.Message)" -ForegroundColor Red
    
    # Check if it's a connection issue
    if ($_.Exception.Message -like "*refused*" -or $_.Exception.Message -like "*timeout*") {
        Write-Host "💡 The API server may not be running on port 5001" -ForegroundColor Yellow
    }
}
