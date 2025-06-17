# Simple API Test Script
$logFile = "c:\Users\seatt\OneDrive\Documents\Projects\brawl-buddy\api-test-results.txt"
$timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"

"=== API Test Results - $timestamp ===" | Out-File $logFile

try {
    $response = Invoke-RestMethod -Uri "http://localhost:5001/api/meta/stats" -Method Get -TimeoutSec 10
    "✅ SUCCESS: Meta stats endpoint responded" | Out-File $logFile -Append
    "Response type: $($response.GetType().Name)" | Out-File $logFile -Append
    
    if ($response.dataSource) {
        "Data source: $($response.dataSource)" | Out-File $logFile -Append
    }
    
    if ($response.Count -gt 0) {
        "Response contains $($response.Count) items" | Out-File $logFile -Append
    }
} catch {
    "❌ FAILED: $($_.Exception.Message)" | Out-File $logFile -Append
}

try {
    $healthResponse = Invoke-WebRequest -Uri "http://localhost:5001" -Method Get -TimeoutSec 5
    "✅ SUCCESS: Server is responding on port 5001 (Status: $($healthResponse.StatusCode))" | Out-File $logFile -Append
} catch {
    "❌ FAILED: Server not responding on port 5001 - $($_.Exception.Message)" | Out-File $logFile -Append
}

"=== Test Complete ===" | Out-File $logFile -Append
