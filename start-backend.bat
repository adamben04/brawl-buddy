@echo off
echo Starting Brawl Buddy Backend...
echo.
cd BrawlBuddy.Api
echo Current directory: %CD%
echo.
echo Running: dotnet run --urls="http://localhost:5000;https://localhost:5001"
dotnet run --urls="http://localhost:5000;https://localhost:5001"
echo.
echo Backend stopped. Press any key to close this window.
pause
