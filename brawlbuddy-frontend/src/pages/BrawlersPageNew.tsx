import { useState, useEffect } from 'react';
import { brawlerApi } from '../services/api';
import { Brawler } from '../types/Brawler';

// Add a function to test CDN connectivity
const testCDNConnectivity = async () => {
  console.log('🌐 Testing CDN connectivity...');
  const testUrl = 'https://cdn.brawlify.com/brawlers/borderless/16000000.png';
  try {
    const response = await fetch(testUrl, { method: 'HEAD' });
    console.log('✅ CDN test successful:', response.status, response.statusText);
    return true;
  } catch (error) {
    console.error('❌ CDN test failed:', error);
    return false;
  }
};

const BrawlersPage = () => {
  const [brawlers, setBrawlers] = useState<Brawler[]>([]);
  const [loading, setLoading] = useState(true);
  const [selectedRarity, setSelectedRarity] = useState<string>('All');
  const [selectedClass, setSelectedClass] = useState<string>('All');  useEffect(() => {
    const fetchBrawlers = async () => {
      try {
        // Test CDN connectivity first
        await testCDNConnectivity();
        
        console.log('🔍 Fetching brawlers from API...');
        const response = await brawlerApi.getAllBrawlers();
        console.log('📦 API Response:', response);
        console.log('🥊 Brawlers data:', response.brawlers);
        console.log('📊 Number of brawlers:', response.brawlers?.length || 0);
        
        if (response.brawlers && response.brawlers.length > 0) {
          console.log('🎯 First brawler sample:', response.brawlers[0]);
        }
        
        setBrawlers(response.brawlers || []);
      } catch (error) {
        console.error('❌ Failed to fetch brawlers:', error);
      } finally {
        setLoading(false);
      }
    };

    fetchBrawlers();
  }, []);
  // Get unique rarities and classes
  const rarities = ['All', ...Array.from(new Set(brawlers.map(b => b.rarity?.name).filter(Boolean)))];
  const classes = ['All', ...Array.from(new Set(brawlers.map(b => b.class?.name).filter(Boolean)))];
  
  console.log('🎭 Available rarities:', rarities);
  console.log('🎯 Available classes:', classes);
  
  // Log sample brawler IDs for debugging
  if (brawlers.length > 0) {
    console.log('🆔 Sample brawler IDs:', brawlers.slice(0, 5).map(b => ({ name: b.name, id: b.id })));
  }
  // Filter brawlers
  const filteredBrawlers = brawlers.filter(brawler => {
    return (selectedRarity === 'All' || brawler.rarity?.name === selectedRarity) &&
           (selectedClass === 'All' || brawler.class?.name === selectedClass);
  });

  console.log('🔢 Total brawlers:', brawlers.length);
  console.log('🎛️ Filtered brawlers:', filteredBrawlers.length);
  console.log('🎨 Selected rarity:', selectedRarity);
  console.log('🏷️ Selected class:', selectedClass);

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
      </div>{/* Filters */}
      <div className="flex flex-wrap gap-4 mb-8 justify-center">
        <div className="flex flex-wrap gap-2">
          <span className="text-white font-brawl-text text-shadow-brawl">Rarity:</span>
          {rarities.map(rarity => (
            <button
              key={rarity}
              onClick={() => setSelectedRarity(rarity)}              className={`px-3 py-1 rounded-full text-sm font-brawl-text transition-all ${
                selectedRarity === rarity
                  ? 'bg-yellow-500 text-black font-brawl-text text-shadow-brawl'
                  : 'bg-white/10 text-gray-300 hover:bg-white/20'
              }`}
            >
              {rarity}
            </button>
          ))}
        </div>
        
        <div className="flex flex-wrap gap-2">
          <span className="text-white font-brawl-text text-shadow-brawl">Class:</span>  
          {classes.map(brawlerClass => (
            <button
              key={brawlerClass}
              onClick={() => setSelectedClass(brawlerClass)}              className={`px-3 py-1 rounded-full text-sm font-brawl-text transition-all ${
                selectedClass === brawlerClass
                  ? 'bg-blue-500 text-white font-brawl-text text-shadow-brawl'
                  : 'bg-white/10 text-gray-300 hover:bg-white/20'
              }`}
            >
              {brawlerClass}
            </button>
          ))}
        </div>
      </div>      {/* Brawler Count */}
      <div className="text-center mb-8">
        <div className="inline-block bg-white/10 backdrop-blur-sm rounded-lg px-6 py-3 border border-white/20">
          <span className="text-2xl font-brawl-title text-yellow-400 text-shadow-brawl">{filteredBrawlers.length}</span>
          <span className="text-gray-300 ml-2 font-brawl-text">
            {selectedRarity === 'All' && selectedClass === 'All' ? 'Total' : 'Filtered'} Brawlers
          </span>
        </div>
      </div>      {/* Loading */}
      {loading && (
        <div className="text-center py-12">
          <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
          <div className="text-white text-lg font-brawl-text text-shadow-brawl">Loading brawlers...</div>
        </div>
      )}

      {/* Brawlers Grid */}
      {!loading && (        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-4">          {filteredBrawlers.map((brawler) => {
            // FIXED: Use the imageUrl directly from API (which is now correct Brawlify URL)
            const imageUrl = brawler.imageUrl || `https://cdn.brawlify.com/brawlers/borderless/${brawler.id}.png`;
            console.log(`�️ Using imageUrl for ${brawler.name} (ID: ${brawler.id}):`, imageUrl);
            
            return (
              <div 
                key={brawler.id} 
                className="bg-white/10 backdrop-blur-sm rounded-lg p-4 border border-white/20 hover:bg-white/20 transition-all duration-200 group cursor-pointer"
              >
                {/* Brawler Image - FIXED to use API imageUrl directly */}
                <div className="w-16 h-16 mx-auto mb-3 flex items-center justify-center relative">
                  <img
                    width="64"
                    height="64"
                    className="opacity-90 rounded-lg transition-transform group-hover:scale-110 object-contain"
                    src={imageUrl}
                    alt={`${brawler.name} is a ${brawler.rarity?.name || 'Unknown'} brawler in Brawl Stars.`}
                    title={`${brawler.name} is a ${brawler.rarity?.name || 'Unknown'} brawler.`}
                    onLoad={() => console.log(`✅ Image loaded successfully for ${brawler.name}:`, imageUrl)}
                    onError={(e) => {
                      console.error(`❌ Failed to load image for ${brawler.name}:`, imageUrl);
                      console.error('Error details:', e);
                      // Show fallback immediately
                      e.currentTarget.style.display = 'none';
                      const fallback = e.currentTarget.nextElementSibling as HTMLElement;
                      if (fallback) {
                        fallback.style.display = 'flex';
                      }
                    }}
                  />
                  {/* Fallback letter circle */}
                  <div 
                    className="w-16 h-16 bg-gradient-to-br from-yellow-400 to-orange-500 rounded-full flex items-center justify-center text-2xl font-bold text-white transition-transform group-hover:scale-110 absolute top-0 left-0"
                    style={{ display: 'none' }}
                  >
                    {brawler.name.charAt(0)}
                  </div>
                </div>                
                {/* Brawler Name */}
                <h3 className="text-white font-brawl-title text-center mb-2 text-sm text-shadow-brawl">
                  {brawler.name}
                </h3>
                
                {/* Rarity Badge */}
                {brawler.rarity?.name && (
                  <div className={`text-xs px-2 py-1 rounded-full text-center mb-2 font-brawl-text ${getRarityColor(brawler.rarity.name)}`}>
                    {brawler.rarity.name}
                  </div>
                )}
                
                {/* Class Badge */}
                {brawler.class?.name && (
                  <div className="text-xs px-2 py-1 rounded-full text-center bg-gray-600/20 text-gray-300 font-brawl-text">
                    {brawler.class.name}
                  </div>
                )}
                
                {/* Star Powers & Gadgets Count */}
                <div className="mt-3 text-xs text-gray-400 space-y-1 font-brawl-text">
                  {brawler.starPowers?.length > 0 && (
                    <div>⭐ {brawler.starPowers.length} Star Power{brawler.starPowers.length !== 1 ? 's' : ''}</div>
                  )}
                  {brawler.gadgets?.length > 0 && (
                    <div>🔧 {brawler.gadgets.length} Gadget{brawler.gadgets.length !== 1 ? 's' : ''}</div>
                  )}
                </div>
              </div>
            );
          })}
        </div>
      )}      {/* No Results */}
      {!loading && filteredBrawlers.length === 0 && (
        <div className="text-center py-12">
          <div className="text-gray-400 text-lg font-brawl-text">No brawlers found with the selected filters.</div>
        </div>
      )}
    </div>
  );
};

export default BrawlersPage;
