import { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { brawlerApi, metaApi } from '../services/api';

const Home = () => {
  const [brawlerCount, setBrawlerCount] = useState<number>(0);
  const [topBrawlers, setTopBrawlers] = useState<any[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchHomeData = async () => {
      try {
        // Fetch brawler count
        const brawlersData = await brawlerApi.getAllBrawlers();
        setBrawlerCount(brawlersData.count);

        // Fetch meta stats for top brawlers
        const metaStats = await metaApi.getMetaStats();
        setTopBrawlers(metaStats.topBrawlers?.slice(0, 6) || []);
      } catch (error) {
        console.error('Failed to fetch home data:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchHomeData();
  }, []);

  return (
    <div className="px-4 sm:px-0">
      {/* Hero Section */}
      <div className="text-center py-16">
        <h1 className="text-6xl font-bold text-white mb-4">
          🥊 <span className="bg-gradient-to-r from-yellow-400 to-orange-500 bg-clip-text text-transparent">
            Brawl Buddy
          </span>
        </h1>
        <p className="text-xl text-gray-300 mb-8 max-w-2xl mx-auto">
          Your ultimate companion for Brawl Stars! Track your stats, analyze battles, 
          and stay ahead of the meta with comprehensive player analytics.
        </p>
        
        {/* Search Bar */}
        <div className="max-w-md mx-auto">
          <div className="relative">
            <input
              type="text"
              placeholder="Enter player tag (e.g., #2PP)"
              className="w-full px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-yellow-400 focus:border-transparent"
              onKeyPress={(e) => {
                if (e.key === 'Enter') {
                  const tag = (e.target as HTMLInputElement).value;
                  window.location.href = `/player/${encodeURIComponent(tag)}`;
                }
              }}
            />
            <div className="absolute inset-y-0 right-0 flex items-center pr-3">
              <svg className="w-5 h-5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
              </svg>
            </div>
          </div>
        </div>
      </div>

      {/* Quick Stats */}
      <div className="grid md:grid-cols-3 gap-6 mb-12">
        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
          <div className="text-3xl font-bold text-yellow-400 mb-2">
            {loading ? '...' : brawlerCount}
          </div>
          <div className="text-gray-300">Total Brawlers</div>
        </div>
        
        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
          <div className="text-3xl font-bold text-green-400 mb-2">Live</div>
          <div className="text-gray-300">API Status</div>
        </div>
        
        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
          <div className="text-3xl font-bold text-blue-400 mb-2">24/7</div>
          <div className="text-gray-300">Data Updates</div>
        </div>
      </div>

      {/* Top Brawlers Preview */}
      {!loading && topBrawlers.length > 0 && (
        <div className="mb-12">
          <h2 className="text-2xl font-bold text-white mb-6">🔥 Top Meta Brawlers</h2>
          <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
            {topBrawlers.map((brawler, index) => (
              <div key={brawler.id} className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20">
                <div className="flex items-center justify-between mb-2">
                  <h3 className="font-semibold text-white">{brawler.name}</h3>
                  <span className="text-sm text-yellow-400">#{index + 1}</span>
                </div>
                <div className="text-sm text-gray-300">
                  Win Rate: <span className="text-green-400">{brawler.winRate}%</span>
                </div>
                <div className="text-sm text-gray-300">
                  Pick Rate: <span className="text-blue-400">{brawler.pickRate}%</span>
                </div>
              </div>
            ))}
          </div>
        </div>
      )}

      {/* Feature Cards */}
      <div className="grid md:grid-cols-2 lg:grid-cols-4 gap-6">
        <Link 
          to="/player" 
          className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 hover:bg-white/20 transition-all duration-200 group"
        >
          <div className="text-4xl mb-4 group-hover:scale-110 transition-transform">👤</div>
          <h3 className="text-lg font-semibold text-white mb-2">Player Stats</h3>
          <p className="text-gray-300 text-sm">Search any player and view detailed statistics, trophy progression, and brawler collection.</p>
        </Link>

        <Link 
          to="/tierlist" 
          className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 hover:bg-white/20 transition-all duration-200 group"
        >
          <div className="text-4xl mb-4 group-hover:scale-110 transition-transform">📊</div>
          <h3 className="text-lg font-semibold text-white mb-2">Tier Lists</h3>
          <p className="text-gray-300 text-sm">Explore current meta tier lists for all game modes and find the best brawlers to play.</p>
        </Link>

        <Link 
          to="/battles" 
          className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 hover:bg-white/20 transition-all duration-200 group"
        >
          <div className="text-4xl mb-4 group-hover:scale-110 transition-transform">⚔️</div>
          <h3 className="text-lg font-semibold text-white mb-2">Battle Analysis</h3>
          <p className="text-gray-300 text-sm">Analyze recent battles, track performance, and identify areas for improvement.</p>
        </Link>

        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 opacity-75">
          <div className="text-4xl mb-4">🗺️</div>
          <h3 className="text-lg font-semibold text-white mb-2">Map Strategies</h3>
          <p className="text-gray-300 text-sm">Coming soon! Get optimal strategies and brawler recommendations for each map.</p>
        </div>
      </div>
    </div>
  );
};

export default Home;