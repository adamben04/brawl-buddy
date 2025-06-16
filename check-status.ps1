Write-Host "=== Brawl Buddy Status Check ===" -ForegroundColor Green
Write-Host ""

# Check if .NET is installed
Write-Host "Checking .NET installation..." -ForegroundColor Yellow
try {
    $dotnetVersion = dotnet --version
    Write-Host "✅ .NET Version: $dotnetVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ .NET not found or not in PATH" -ForegroundColor Red
}
Write-Host ""

# Check if Node.js is installed
Write-Host "Checking Node.js installation..." -ForegroundColor Yellow
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js Version: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js not found or not in PATH" -ForegroundColor Red
}
Write-Host ""

# Check if ports are in use
Write-Host "Checking ports..." -ForegroundColor Yellow
$port5000 = netstat -ano | Select-String ":5000"
$port5001 = netstat -ano | Select-String ":5001"
$port5173 = netstat -ano | Select-String ":5173"

if ($port5000) {
    Write-Host "✅ Port 5000 is in use (Backend HTTP)" -ForegroundColor Green
} else {
    Write-Host "❌ Port 5000 is not in use" -ForegroundColor Red
}

if ($port5001) {
    Write-Host "✅ Port 5001 is in use (Backend HTTPS)" -ForegroundColor Green
} else {
    Write-Host "❌ Port 5001 is not in use" -ForegroundColor Red
}

if ($port5173) {
    Write-Host "✅ Port 5173 is in use (Frontend)" -ForegroundColor Green
} else {
    Write-Host "❌ Port 5173 is not in use" -ForegroundColor Red
}
Write-Host ""

# Test backend endpoints
Write-Host "Testing backend endpoints..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/player/test" -Method GET -TimeoutSec 5
    Write-Host "✅ Backend HTTP endpoint working: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend HTTP endpoint not responding" -ForegroundColor Red
}

try {
    $response = Invoke-RestMethod -Uri "https://localhost:5001/api/player/test" -Method GET -TimeoutSec 5 -SkipCertificateCheck
    Write-Host "✅ Backend HTTPS endpoint working: $($response.message)" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend HTTPS endpoint not responding" -ForegroundColor Red
}
Write-Host ""

Write-Host "=== Access URLs ===" -ForegroundColor Cyan
Write-Host "Frontend:      http://localhost:5173" -ForegroundColor White
Write-Host "Backend HTTP:  http://localhost:5000" -ForegroundColor White
Write-Host "Backend HTTPS: https://localhost:5001" -ForegroundColor White
Write-Host "Swagger UI:    http://localhost:5000/swagger" -ForegroundColor White
Write-Host "Test Endpoint: http://localhost:5000/api/player/test" -ForegroundColor White
Write-Host ""

Read-Host "Press Enter to continue..."
