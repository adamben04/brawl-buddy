import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import { useState, useEffect } from 'react';
import { playerApi } from './services/api';

// Import pages
import Home from './pages/Home';
import PlayerProfile from './pages/PlayerProfile';
import TierListPage from './pages/TierListPage';
import BattleLogPage from './pages/BattleLogPage';

function App() {
  const [backendStatus, setBackendStatus] = useState<'pending' | 'success' | 'error'>('pending');

  useEffect(() => {
    // Test backend connection on app load
    const testBackend = async () => {
      try {
        await playerApi.test();
        setBackendStatus('success');
      } catch (error) {
        setBackendStatus('error');
        console.error('Backend connection failed:', error);
      }
    };

    testBackend();
  }, []);

  return (
    <Router>
      <div className="min-h-screen bg-gradient-to-br from-blue-900 via-purple-900 to-indigo-900">
        {/* Navigation Header */}
        <nav className="bg-black/20 backdrop-blur-sm border-b border-white/10">
          <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
            <div className="flex items-center justify-between h-16">
              <div className="flex items-center">
                <Link to="/" className="text-white text-xl font-bold">
                  🥊 Brawl Buddy
                </Link>
              </div>
              <div className="flex space-x-4">
                <Link 
                  to="/" 
                  className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Home
                </Link>
                <Link 
                  to="/player" 
                  className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Player Search
                </Link>
                <Link 
                  to="/tierlist" 
                  className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Tier Lists
                </Link>
                <Link 
                  to="/battles" 
                  className="text-gray-300 hover:text-white px-3 py-2 rounded-md text-sm font-medium transition-colors"
                >
                  Battle Log
                </Link>
              </div>
              
              {/* Backend Status Indicator */}
              <div className="flex items-center">
                <div className={`w-2 h-2 rounded-full mr-2 ${
                  backendStatus === 'success' ? 'bg-green-400' : 
                  backendStatus === 'error' ? 'bg-red-400' : 'bg-yellow-400'
                }`}></div>
                <span className="text-xs text-gray-400">
                  {backendStatus === 'success' ? 'API Connected' : 
                   backendStatus === 'error' ? 'API Offline' : 'Connecting...'}
                </span>
              </div>
            </div>
          </div>
        </nav>

        {/* Main Content */}
        <main className="max-w-7xl mx-auto py-6 sm:px-6 lg:px-8">
          {backendStatus === 'error' && (
            <div className="bg-red-900/50 border border-red-400 text-red-100 px-4 py-3 rounded mb-6">
              <strong className="font-bold">Backend Offline!</strong>
              <span className="block sm:inline"> Make sure the .NET API is running on localhost:5000</span>
            </div>
          )}
          
          <Routes>
            <Route path="/" element={<Home />} />
            <Route path="/player" element={<PlayerProfile />} />
            <Route path="/player/:tag" element={<PlayerProfile />} />
            <Route path="/tierlist" element={<TierListPage />} />
            <Route path="/battles" element={<BattleLogPage />} />
            <Route path="/battles/:tag" element={<BattleLogPage />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;