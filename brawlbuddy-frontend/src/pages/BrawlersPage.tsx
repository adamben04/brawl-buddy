import { useState, useEffect } from 'react';
import { brawlerApi } from '../services/api';
import { Brawler } from '../types/Brawler';

const BrawlersPage = () => {
  const [brawlers, setBrawlers] = useState<Brawler[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedRarity, setSelectedRarity] = useState<string>('All');
  const [selectedClass, setSelectedClass] = useState<string>('All');

  useEffect(() => {
    const fetchBrawlers = async () => {
      try {
        const response = await brawlerApi.getAllBrawlers();
        setBrawlers(response.brawlers || []);
      } catch (error) {
        console.error('Failed to fetch brawlers:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBrawlers();
  }, []);

  // Get unique rarities and classes
  const rarities = ['All', ...Array.from(new Set(brawlers.map(b => b.rarity?.name).filter(Boolean)))];
  const classes = ['All', ...Array.from(new Set(brawlers.map(b => b.class?.name).filter(Boolean)))];

  // Filter brawlers
  const filteredBrawlers = brawlers.filter(brawler => {
    return (selectedRarity === 'All' || brawler.rarity?.name === selectedRarity) &&
           (selectedClass === 'All' || brawler.class?.name === selectedClass);
  });

  const getRarityColor = (rarity: string) => {
    switch (rarity?.toLowerCase()) {
      case 'common': return 'text-gray-400 bg-gray-500/20';
      case 'rare': return 'text-green-400 bg-green-500/20';
      case 'super rare': return 'text-blue-400 bg-blue-500/20';
      case 'epic': return 'text-purple-400 bg-purple-500/20';
      case 'mythic': return 'text-pink-400 bg-pink-500/20';
      case 'legendary': return 'text-yellow-400 bg-yellow-500/20';
      case 'ultra legendary': return 'text-orange-400 bg-orange-500/20';
      default: return 'text-gray-400 bg-gray-500/20';
    }
  };

  return (
    <div className="px-4 sm:px-0">      {/* Header */}
      <div className="text-center py-8">
        <h1 className="text-5xl font-brawl-title text-white mb-4 text-shadow-brawl">
          🥊 <span className="text-yellow-400">ALL BRAWLERS</span>
        </h1>
        <p className="text-xl font-brawl-text text-gray-300 mb-6 text-shadow-brawl">
          Master every fighter in Brawl Stars! Check their stats and learn more about them.
        </p>
      </div>

      {/* Filters */}
      <div className="flex flex-wrap gap-4 mb-8 justify-center">
        <div className="flex flex-wrap gap-2">
          <span className="text-white font-medium">Rarity:</span>
          {rarities.map(rarity => (
            <button
              key={rarity}
              onClick={() => setSelectedRarity(rarity)}
              className={`px-3 py-1 rounded-full text-sm transition-all ${
                selectedRarity === rarity
                  ? 'bg-yellow-500 text-black font-medium'
                  : 'bg-white/10 text-gray-300 hover:bg-white/20'
              }`}
            >
              {rarity}
            </button>
          ))}
        </div>
        
        <div className="flex flex-wrap gap-2">
          <span className="text-white font-medium">Class:</span>  
          {classes.map(brawlerClass => (
            <button
              key={brawlerClass}
              onClick={() => setSelectedClass(brawlerClass)}
              className={`px-3 py-1 rounded-full text-sm transition-all ${
                selectedClass === brawlerClass
                  ? 'bg-blue-500 text-white font-medium'
                  : 'bg-white/10 text-gray-300 hover:bg-white/20'
              }`}
            >
              {brawlerClass}
            </button>
          ))}
        </div>
      </div>

      {/* Brawler Count */}
      <div className="text-center mb-8">
        <div className="inline-block bg-white/10 backdrop-blur-sm rounded-lg px-6 py-3 border border-white/20">
          <span className="text-2xl font-bold text-yellow-400">{filteredBrawlers.length}</span>
          <span className="text-gray-300 ml-2">
            {selectedRarity === 'All' && selectedClass === 'All' ? 'Total' : 'Filtered'} Brawlers
          </span>
        </div>
      </div>

      {/* Loading */}
      {loading && (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
          <div className="text-white text-lg">Loading brawlers...</div>
        </div>
      )}

      {/* Brawlers Grid */}
      {!loading && (
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-4">
          {filteredBrawlers.map((brawler) => (
            <div 
              key={brawler.id} 
              className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20 hover:bg-white/20 transition-all duration-200 group cursor-pointer"
            >
              {/* Brawler Image Placeholder */}
              <div className="w-16 h-16 mx-auto mb-3 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-full flex items-center justify-center text-2xl font-bold text-white group-hover:scale-110 transition-transform">
                {brawler.name.charAt(0)}
              </div>
              
              {/* Brawler Name */}
              <h3 className="text-white font-semibold text-center mb-2 text-sm">
                {brawler.name}
              </h3>
              
              {/* Rarity Badge */}
              {brawler.rarity?.name && (
                <div className={`text-xs px-2 py-1 rounded-full text-center mb-2 ${getRarityColor(brawler.rarity.name)}`}>
                  {brawler.rarity.name}
                </div>
              )}
              
              {/* Class Badge */}
              {brawler.class?.name && (
                <div className="text-xs px-2 py-1 rounded-full text-center bg-gray-600/20 text-gray-300">
                  {brawler.class.name}
                </div>
              )}
              
              {/* Star Powers & Gadgets Count */}
              <div className="mt-3 text-xs text-gray-400 space-y-1">
                {brawler.starPowers?.length > 0 && (
                  <div>⭐ {brawler.starPowers.length} Star Power{brawler.starPowers.length !== 1 ? 's' : ''}</div>
                )}
                {brawler.gadgets?.length > 0 && (
                  <div>🔧 {brawler.gadgets.length} Gadget{brawler.gadgets.length !== 1 ? 's' : ''}</div>
                )}
              </div>
            </div>
          ))}
        </div>
      )}

      {/* No Results */}
      {!loading && filteredBrawlers.length === 0 && (
        <div className="text-center py-12">
          <div className="text-gray-400 text-lg">No brawlers found with the selected filters.</div>
        </div>
      )}
    </div>
  );
};

export default BrawlersPage;
