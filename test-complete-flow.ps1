# Complete flow test - Frontend to Backend integration
Write-Host "=== COMPLETE FLOW TEST ===" -ForegroundColor Green

# Test 1: Backend API directly
Write-Host "`nTesting Backend API directly..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://127.0.0.1:5001/api/meta/brawlers/count" -Method GET
    $data = $response.Content | ConvertFrom-Json
    Write-Host "✅ Backend API Response:" -ForegroundColor Green
    Write-Host "   Count: $($data.count)" -ForegroundColor Cyan
    Write-Host "   Data Source: $($data.dataSource)" -ForegroundColor Cyan
    Write-Host "   Message: $($data.message)" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Backend API Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 2: Check if frontend is running
Write-Host "`nTesting Frontend availability..." -ForegroundColor Yellow
try {
    $frontendResponse = Invoke-WebRequest -Uri "http://localhost:5174" -Method GET
    Write-Host "✅ Frontend is accessible (Status: $($frontendResponse.StatusCode))" -ForegroundColor Green
} catch {
    Write-Host "❌ Frontend Error: $($_.Exception.Message)" -ForegroundColor Red
}

# Test 3: Test other backend endpoints
Write-Host "`nTesting other backend endpoints..." -ForegroundColor Yellow

$endpoints = @(
    "/api/meta/info",
    "/api/brawler"
)

foreach ($endpoint in $endpoints) {
    try {
        $response = Invoke-WebRequest -Uri "http://127.0.0.1:5001$endpoint" -Method GET
        Write-Host "✅ $endpoint - Status: $($response.StatusCode)" -ForegroundColor Green
    } catch {
        Write-Host "❌ $endpoint - Error: $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host "`n=== TEST COMPLETE ===" -ForegroundColor Green
Write-Host "Frontend URL: http://localhost:5174" -ForegroundColor Cyan
Write-Host "Backend Swagger: http://localhost:5001/swagger (if HTTPS redirect is disabled)" -ForegroundColor Cyan
