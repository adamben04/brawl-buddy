import { useState, useEffect } from 'react';

interface MapData {
  id: string;
  name: string;
  mode: string;
  battleCount?: number;
  topBrawlers?: Array<{
    name: string;
    winRate: number;
    useRate: number;
  }>;
}

const MapsPage = () => {
  const [maps, setMaps] = useState<MapData[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedMode, setSelectedMode] = useState<string>('All');

  // Mock data for now - replace with actual API call when available
  useEffect(() => {
    const fetchMaps = async () => {
      try {
        // For now, create mock map data similar to Brawlify
        const mockMaps: MapData[] = [
          // Gem Grab
          { id: '1', name: 'Hard Rock Mine', mode: 'Gem Grab', battleCount: 1500000 },
          { id: '2', name: 'Crystal Arcade', mode: 'Gem Grab', battleCount: 1200000 },
          { id: '3', name: 'Deathcap Trap', mode: 'Gem Grab', battleCount: 800000 },
          { id: '4', name: 'On A Roll', mode: 'Gem Grab', battleCount: 950000 },
          
          // Heist
          { id: '5', name: 'Kaboom Canyon', mode: 'Heist', battleCount: 700000 },
          { id: '6', name: 'Safe Zone', mode: 'Heist', battleCount: 600000 },
          { id: '7', name: 'Hot Potato', mode: 'Heist', battleCount: 550000 },
          
          // Bounty
          { id: '8', name: 'Shooting Star', mode: 'Bounty', battleCount: 400000 },
          { id: '9', name: 'Layer Cake', mode: 'Bounty', battleCount: 350000 },
          { id: '10', name: 'Dry Season', mode: 'Bounty', battleCount: 300000 },
          
          // Brawl Ball
          { id: '11', name: 'Backyard Bowl', mode: 'Brawl Ball', battleCount: 2000000 },
          { id: '12', name: 'Sneaky Fields', mode: 'Brawl Ball', battleCount: 1800000 },
          { id: '13', name: 'Beach Ball', mode: 'Brawl Ball', battleCount: 1600000 },
          { id: '14', name: 'Spiraling Out', mode: 'Brawl Ball', battleCount: 1400000 },
          
          // Solo Showdown
          { id: '15', name: 'Skull Creek', mode: 'Solo Showdown', battleCount: 3000000 },
          { id: '16', name: 'Feast Or Famine', mode: 'Solo Showdown', battleCount: 2800000 },
          { id: '17', name: 'Cavern Churn', mode: 'Solo Showdown', battleCount: 2600000 },
          { id: '18', name: 'Acid Lakes', mode: 'Solo Showdown', battleCount: 2400000 },
          
          // Hot Zone
          { id: '19', name: 'Ring Of Fire', mode: 'Hot Zone', battleCount: 500000 },
          { id: '20', name: 'Bejeweled', mode: 'Hot Zone', battleCount: 450000 },
          
          // Knockout
          { id: '21', name: 'Goldarm Gulch', mode: 'Knockout', battleCount: 800000 },
          { id: '22', name: 'Double Decker', mode: 'Knockout', battleCount: 700000 },
          { id: '23', name: 'Close Quarters', mode: 'Knockout', battleCount: 600000 },
        ];
        setMaps(mockMaps);
      } catch (error) {
        console.error('Failed to fetch maps:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchMaps();
  }, []);

  // Get unique game modes
  const gameModes = ['All', ...Array.from(new Set(maps.map(m => m.mode)))];

  // Filter maps by mode
  const filteredMaps = maps.filter(map => 
    selectedMode === 'All' || map.mode === selectedMode
  );

  // Group maps by mode for display
  const mapsByMode = filteredMaps.reduce((acc, map) => {
    if (!acc[map.mode]) {
      acc[map.mode] = [];
    }
    acc[map.mode].push(map);
    return acc;
  }, {} as { [mode: string]: MapData[] });

  const getModeIcon = (mode: string) => {
    switch (mode) {
      case 'Gem Grab': return '💎';
      case 'Heist': return '💰';
      case 'Bounty': return '⭐';
      case 'Brawl Ball': return '⚽';
      case 'Solo Showdown': return '🏟️';
      case 'Duo Showdown': return '👥';
      case 'Hot Zone': return '🔥';
      case 'Knockout': return '🥊';
      case 'Duels': return '⚔️';
      default: return '🗺️';
    }
  };

  const getModeColor = (mode: string) => {
    switch (mode) {
      case 'Gem Grab': return 'font-brawl-text bg-green-500/20 text-green-400';
      case 'Heist': return 'font-brawl-text bg-yellow-500/20 text-yellow-400';
      case 'Bounty': return 'font-brawl-text bg-blue-500/20 text-blue-400';
      case 'Brawl Ball': return 'font-brawl-text bg-orange-500/20 text-orange-400';
      case 'Solo Showdown': return 'font-brawl-text bg-red-500/20 text-red-400';
      case 'Duo Showdown': return 'font-brawl-text bg-purple-500/20 text-purple-400';
      case 'Hot Zone': return 'font-brawl-text bg-pink-500/20 text-pink-400';
      case 'Knockout': return 'font-brawl-text bg-gray-500/20 text-gray-400';
      default: return 'font-brawl-text bg-gray-500/20 text-gray-400';
    }
  };

  return (
    <div className="px-4 sm:px-0">      {/* Header */}
      <div className="text-center py-8">
        <h1 className="text-5xl font-brawl-title text-white mb-4 text-shadow-brawl">
          🗺️ <span className="text-yellow-400">ALL MAPS</span>
        </h1>
        <p className="text-xl font-brawl-text text-gray-300 mb-6 text-shadow-brawl">
          Discover every battlefield in Brawl Stars! Find the best maps for your favorite brawlers and dominate the competition.
        </p>
      </div>      {/* Mode Filter */}
      <div className="flex flex-wrap gap-2 mb-8 justify-center">
        <span className="font-brawl-title text-white text-lg mr-4 text-shadow-brawl">🎮 GAME MODE:</span>
        {gameModes.map(mode => (
          <button
            key={mode}
            onClick={() => setSelectedMode(mode)}
            className={`px-4 py-2 rounded-full text-sm font-brawl-text transition-all flex items-center gap-2 text-shadow-brawl ${
              selectedMode === mode
                ? 'bg-yellow-500 text-black font-brawl-title'
                : 'bg-white/10 text-gray-300 hover:bg-white/20'
            }`}
          >
            {mode !== 'All' && getModeIcon(mode)}
            {mode.toUpperCase()}
            {mode !== 'All' && (
              <span className="text-xs opacity-75">
                ({mapsByMode[mode]?.length || 0})
              </span>
            )}
          </button>
        ))}
      </div>      {/* Loading */}
      {loading && (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
          <div className="font-brawl-title text-white text-xl text-shadow-brawl">⚡ LOADING BATTLE MAPS...</div>
        </div>
      )}

      {/* Maps by Mode */}
      {!loading && Object.keys(mapsByMode).map(mode => (
        <div key={mode} className="mb-12">          <div className="flex items-center gap-3 mb-6">
            <div className={`px-3 py-1 rounded-full text-sm font-brawl-title ${getModeColor(mode)}`}>
              {getModeIcon(mode)} {mode.toUpperCase()}
            </div>
            <span className="font-brawl-text text-gray-400">({mapsByMode[mode].length} battlefields)</span>
          </div>
          
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4">
            {mapsByMode[mode].map(map => (
              <div 
                key={map.id}
                className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20 hover:bg-white/20 transition-all duration-200 cursor-pointer group"
              >
                {/* Map Image Placeholder */}
                <div className="w-full h-32 bg-gradient-to-br from-gray-600 to-gray-800 rounded-lg mb-3 flex items-center justify-center text-4xl group-hover:scale-105 transition-transform">
                  {getModeIcon(map.mode)}
                </div>
                  {/* Map Name */}
                <h3 className="font-brawl-title text-white text-lg mb-2 text-shadow-brawl">{map.name}</h3>
                
                {/* Mode Badge */}
                <div className={`inline-block px-2 py-1 rounded-full text-xs mb-2 font-brawl-text ${getModeColor(map.mode)}`}>
                  {map.mode.toUpperCase()}
                </div>
                
                {/* Battle Count */}
                {map.battleCount && (
                  <div className="text-sm font-brawl-text text-gray-400">
                    ⚔️ {map.battleCount.toLocaleString()} battles
                  </div>
                )}
                
                {/* Coming Soon indicator */}
                <div className="mt-2 text-sm font-brawl-text text-yellow-400 text-shadow-brawl">
                  📊 STATS COMING SOON
                </div>
              </div>
            ))}
          </div>
        </div>
      ))}      {/* No Results */}
      {!loading && Object.keys(mapsByMode).length === 0 && (
        <div className="text-center py-12">
          <div className="font-brawl-title text-gray-400 text-xl text-shadow-brawl">🚫 NO BATTLEFIELDS FOUND</div>
          <p className="font-brawl-text text-gray-500 mt-2">Try selecting a different game mode!</p>
        </div>
      )}

      {/* Shortcuts Section */}
      <div className="mt-12 bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
        <h3 className="font-brawl-title text-white text-xl mb-4 text-shadow-brawl">🚀 QUICK NAVIGATION</h3>
        <div className="flex flex-wrap gap-2">
          {gameModes.slice(1).map(mode => (
            <button
              key={mode}
              onClick={() => setSelectedMode(mode)}
              className="px-3 py-1 rounded-full text-sm font-brawl-text bg-white/10 text-gray-300 hover:bg-white/20 transition-all"
            >
              {getModeIcon(mode)} {mode.toUpperCase()} ({mapsByMode[mode]?.length || 0})
            </button>
          ))}
        </div>
      </div>
    </div>
  );
};

export default MapsPage;
