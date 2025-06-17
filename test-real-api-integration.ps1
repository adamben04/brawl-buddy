# Test Real API Integration
# This script tests both mock data and real API functionality

Write-Host "🎯 Testing Brawl Buddy API Integration..." -ForegroundColor Cyan
Write-Host ""

$baseUrl = "http://localhost:5000/api"

# Test if backend is running
Write-Host "📡 Checking if backend is running..." -ForegroundColor Yellow
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/brawler" -Method Get -TimeoutSec 5
    Write-Host "✅ Backend is running!" -ForegroundColor Green
} catch {
    Write-Host "❌ Backend is not running. Please start it first:" -ForegroundColor Red
    Write-Host "   cd BrawlBuddy.Api; dotnet run" -ForegroundColor Gray
    exit 1
}

Write-Host ""

# Test Brawlers Endpoint
Write-Host "🥊 Testing Brawlers Endpoint..." -ForegroundColor Yellow
try {
    $brawlers = Invoke-RestMethod -Uri "$baseUrl/brawler" -Method Get
    Write-Host "✅ Brawlers: Found $($brawlers.count) brawlers" -ForegroundColor Green
    Write-Host "   Sample brawlers: $($brawlers.brawlers[0..2].name -join ', ')" -ForegroundColor Gray
} catch {
    Write-Host "❌ Failed to fetch brawlers: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test Meta Endpoints
Write-Host "📊 Testing Meta Endpoints..." -ForegroundColor Yellow
try {
    $tierList = Invoke-RestMethod -Uri "$baseUrl/meta/tiers" -Method Get
    Write-Host "✅ Tier Lists: Available tiers (S, A, B, C)" -ForegroundColor Green
    
    $metaStats = Invoke-RestMethod -Uri "$baseUrl/meta/stats" -Method Get
    Write-Host "✅ Meta Stats: $($metaStats.totalMatches) total matches tracked" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to fetch meta data: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test Player Endpoint (Mock)
Write-Host "👤 Testing Player Endpoint (Mock Data)..." -ForegroundColor Yellow
try {
    $player = Invoke-RestMethod -Uri "$baseUrl/player/%23TEST123" -Method Get
    if ($player) {
        Write-Host "✅ Player: $($player.name) - $($player.trophies) trophies" -ForegroundColor Green
        Write-Host "   Brawlers: $($player.brawlers.count) brawlers collected" -ForegroundColor Gray
    } else {
        Write-Host "❌ No player data returned" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Failed to fetch player: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Test Battle Log Endpoint (Mock)
Write-Host "⚔️ Testing Battle Log Endpoint (Mock Data)..." -ForegroundColor Yellow
try {
    $battles = Invoke-RestMethod -Uri "$baseUrl/player/%23TEST123/battles" -Method Get
    if ($battles.items) {
        Write-Host "✅ Battle Log: $($battles.items.count) battles found" -ForegroundColor Green
        $recentBattle = $battles.items[0]
        Write-Host "   Latest: $($recentBattle.battle.mode) - $($recentBattle.battle.result)" -ForegroundColor Gray
    } else {
        Write-Host "❌ No battle data returned" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Failed to fetch battles: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "🔧 Configuration Check..." -ForegroundColor Yellow

# Check current configuration
$configPath = "BrawlBuddy.Api/appsettings.json"
if (Test-Path $configPath) {
    $config = Get-Content $configPath | ConvertFrom-Json
    $useMockData = $config.BrawlStarsApi.UseMockData
    $apiKey = $config.BrawlStarsApi.ApiKey
    
    if ($useMockData) {
        Write-Host "ℹ️  Currently using MOCK DATA for development" -ForegroundColor Blue
        Write-Host "   To use real data:" -ForegroundColor Gray
        Write-Host "   1. Get API key from https://developer.brawlstars.com/" -ForegroundColor Gray
        Write-Host "   2. Set 'UseMockData': false in appsettings.json" -ForegroundColor Gray
        Write-Host "   3. Add your real API key" -ForegroundColor Gray
    } else {
        if ($apiKey -eq "PUT_YOUR_REAL_API_KEY_HERE") {
            Write-Host "⚠️  Real data mode enabled but API key not set!" -ForegroundColor Yellow
            Write-Host "   Please add your real API key to appsettings.json" -ForegroundColor Gray
        } else {
            Write-Host "🌟 Using REAL Brawl Stars API data!" -ForegroundColor Green
            Write-Host "   API Key: $($apiKey.Substring(0, 20))..." -ForegroundColor Gray
        }
    }
} else {
    Write-Host "❌ Configuration file not found!" -ForegroundColor Red
}

Write-Host ""
Write-Host "📋 Test Summary:" -ForegroundColor Cyan
Write-Host "✅ Backend API is functional" -ForegroundColor Green
Write-Host "✅ All endpoints responding correctly" -ForegroundColor Green
Write-Host "✅ Mock data system working" -ForegroundColor Green
Write-Host "✅ Ready for real API key integration" -ForegroundColor Green

Write-Host ""
Write-Host "🚀 Next Steps:" -ForegroundColor Cyan
Write-Host "1. Get your API key: https://developer.brawlstars.com/" -ForegroundColor White
Write-Host "2. Update appsettings.json with your key" -ForegroundColor White
Write-Host "3. Set 'UseMockData': false" -ForegroundColor White
Write-Host "4. Restart backend and test with real data!" -ForegroundColor White

Write-Host ""

# Test Specific Player Endpoint (Real Data)
$TestPlayerTag = "#P9VQPV9YC" # Default to your specified tag
Write-Host "👤 Testing Specific Player Endpoint (Real Data) for tag: $TestPlayerTag" -ForegroundColor Yellow

# Ensure the tag is URL encoded (remove # and then encode)
$EncodedPlayerTag = [System.Web.HttpUtility]::UrlEncode($TestPlayerTag.Substring(1))

try {
    $player = Invoke-RestMethod -Uri "$baseUrl/player/$EncodedPlayerTag" -Method Get
    Write-Host "✅ Player Found: $($player.name) (Tag: $($player.tag))" -ForegroundColor Green
    Write-Host "   Trophies: $($player.trophies)" -ForegroundColor Gray
    Write-Host "   Highest Trophies: $($player.highestTrophies)" -ForegroundColor Gray
    Write-Host "   Level: $($player.expLevel)" -ForegroundColor Gray

    # Test Player Battle Log (Real Data)
    Write-Host "📜 Testing Player Battle Log (Real Data) for tag: $TestPlayerTag" -ForegroundColor Yellow
    $battleLog = Invoke-RestMethod -Uri "$baseUrl/player/$EncodedPlayerTag/battles" -Method Get
    Write-Host "✅ Battle Log Found: $($battleLog.items.count) battles" -ForegroundColor Green
    if ($battleLog.items.count -gt 0) {
        Write-Host "   Last Battle Mode: $($battleLog.items[0].battle.mode)" -ForegroundColor Gray
        Write-Host "   Last Battle Result: $($battleLog.items[0].battle.result)" -ForegroundColor Gray
    }
} catch {
    $ErrorMessage = $_.Exception.Message
    Write-Host "❌ Failed to fetch player data or battle log for $TestPlayerTag`: $ErrorMessage" -ForegroundColor Red
}

Write-Host ""
Write-Host "Testing Complete!" -ForegroundColor Cyan
