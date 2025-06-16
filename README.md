# 🎮 Brawl Buddy

Your ultimate Brawl Stars companion! Brawl Buddy helps you track player statistics, analyze battle logs, discover optimal strategies, and stay updated with the latest meta trends.

## ✨ Features

- **Player Profile Tracking**: View detailed player statistics and progression
- **Battle Log Analysis**: Analyze your recent matches and performance
- **Brawler Information**: Complete database of all brawlers with stats and abilities
- **Meta Insights**: Stay updated with current tier lists and meta trends
- **Strategy Guides**: Map-specific strategies and tips
- **Event Rotation Tracking**: Keep track of current and upcoming events

## 🛠️ Tech Stack

### Backend (.NET 8 Web API)
- **Framework**: ASP.NET Core 8.0
- **Documentation**: Swagger/OpenAPI
- **Architecture**: RESTful API with clean architecture principles

### Frontend (React + TypeScript)
- **Framework**: React 18 with TypeScript
- **Build Tool**: Vite
- **Styling**: Tailwind CSS
- **HTTP Client**: Axios

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js 18+](https://nodejs.org/)
- [Git](https://git-scm.com/)

### Quick Start

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/brawl-buddy.git
   cd brawl-buddy
   ```

2. **Start the Backend API**
   ```bash
   # Option 1: Using the batch file
   start-backend.bat
   
   # Option 2: Manual start
   cd BrawlBuddy.Api
   dotnet run --urls="http://localhost:5000;https://localhost:5001"
   ```

3. **Start the Frontend**
   ```bash
   # Option 1: Using the batch file
   start-frontend.bat
   
   # Option 2: Manual start
   cd brawlbuddy-frontend
   npm install
   npm run dev
   ```

4. **Access the Application**
   - Frontend: http://localhost:5174
   - Backend API: http://localhost:5000
   - Swagger Documentation: http://localhost:5000/swagger

### Alternative Setup Methods

#### Using PowerShell Scripts
```powershell
# Start backend
.\start-backend.ps1

# Check system status
.\check-status.ps1
```

#### Using VS Code Tasks
1. Open the project in VS Code
2. Press `Ctrl+Shift+P` → "Tasks: Run Task"
3. Select "Start Backend API"

## 📁 Project Structure

```
brawl-buddy/
├── BrawlBuddy.Api/              # .NET Backend API
│   ├── Controllers/             # API Controllers
│   ├── Models/                  # Data Models
│   ├── Services/                # Business Logic
│   └── Program.cs               # Application Entry Point
├── brawlbuddy-frontend/         # React Frontend
│   ├── src/
│   │   ├── components/          # Reusable Components
│   │   ├── pages/               # Page Components
│   │   ├── services/            # API Services
│   │   ├── types/               # TypeScript Definitions
│   │   └── utils/               # Utility Functions
│   └── public/                  # Static Assets
├── .gitignore                   # Git Ignore Rules
├── README.md                    # Project Documentation
└── DEVELOPMENT_GUIDE.md         # Development Setup Guide
```

## 🔧 Development

### Backend Development
- Built with ASP.NET Core 8.0
- Follows REST API conventions
- Includes Swagger documentation
- Configured for both HTTP and HTTPS

### Frontend Development
- React 18 with TypeScript for type safety
- Vite for fast development and building
- Tailwind CSS for styling
- Axios for HTTP requests

### Building for Production

#### Frontend
```bash
cd brawlbuddy-frontend
npm run build
```

#### Backend
```bash
cd BrawlBuddy.Api
dotnet publish -c Release
```

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 📝 API Documentation

The API documentation is available at `/swagger` when running the backend:
- Local: http://localhost:5000/swagger
- Includes all available endpoints and data models
- Interactive testing interface

## 🐛 Troubleshooting

### Common Issues

1. **Port Already in Use**
   ```bash
   # Check what's using the port
   netstat -ano | findstr :5000
   
   # Kill the process
   taskkill /PID <PID_NUMBER> /F
   ```

2. **PowerShell Execution Policy**
   ```powershell
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

3. **Node.js/npm Issues**
   ```bash
   # Clear npm cache
   npm cache clean --force
   
   # Reinstall dependencies
   rm -rf node_modules package-lock.json
   npm install
   ```

For more detailed setup instructions, see [DEVELOPMENT_GUIDE.md](./DEVELOPMENT_GUIDE.md).

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🎯 Roadmap

- [ ] Player search and profile viewing
- [ ] Battle log analysis with statistics
- [ ] Brawler tier list and meta analysis
- [ ] Map-specific strategy guides
- [ ] Event rotation tracking
- [ ] Push notifications for new events
- [ ] Mobile responsive design
- [ ] Dark/light theme toggle

## 🙏 Acknowledgments

- Supercell for creating Brawl Stars
- The Brawl Stars community for inspiration
- All contributors who help improve this project

---

**Built with ❤️ for the Brawl Stars community**
