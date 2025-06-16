# Brawl Buddy - Local Development Setup

# Brawl Buddy - Local Development Setup

## Quick Start

### Option 1: Using VS Code Tasks (Recommended)
1. Open the project in VS Code
2. Press `Ctrl+Shift+P` and type "Tasks: Run Task"
3. Select "Start Backend API" to start the backend
4. In a separate terminal, navigate to `brawlbuddy-frontend` and run `npm run dev`

### Option 2: Using Batch Files
1. **Start Backend**: Double-click `start-backend.bat` in the project root
2. **Start Frontend**: Double-click `start-frontend.bat` in the project root

### Option 3: Using PowerShell Scripts
1. **Start Backend**: Right-click `start-backend.ps1` and "Run with PowerShell"
2. **Check Status**: Right-click `check-status.ps1` and "Run with PowerShell"

### Option 4: Manual Commands

#### Backend (API)
1. Open PowerShell/Command Prompt
2. Navigate to: `cd BrawlBuddy.Api`
3. Run: `dotnet run --urls="http://localhost:5000;https://localhost:5001"`

#### Frontend (React)
1. Open another PowerShell/Command Prompt
2. Navigate to: `cd brawlbuddy-frontend`
3. Run: `npm run dev`

## Access Points

- **Frontend**: http://localhost:5174
- **Backend API**: http://localhost:5000
- **Swagger UI**: http://localhost:5000/swagger
- **Test Endpoint**: http://localhost:5000/api/player/test

## Troubleshooting

1. **If npm commands don't work**: Run this first:
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

2. **If ports are in use**: 
   - Check what's using the ports: `netstat -ano | findstr :5000`
   - Kill the process: `taskkill /PID <PID_NUMBER> /F`

3. **CORS Issues**: The backend is configured to allow requests from:
   - http://localhost:5174
   - http://localhost:5173
   - http://localhost:3000

## Current Status
- ✅ Backend API with CORS configured
- ✅ Frontend React app with Vite
- ✅ Test endpoint to verify connection
- ✅ Tailwind CSS for styling
- ✅ TypeScript support

The frontend will automatically test the backend connection and display the status on the page.
