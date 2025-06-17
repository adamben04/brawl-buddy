import { useState, useEffect } from 'react';
import { metaApi } from '../services/api';

interface TopBrawler {
  id: number;
  name: string;
  winRate: number;
  pickRate: number;
  trend?: string;
  rank: number;
}

interface DailyMetaEvent {
  mode: string;
  map: string;
  timeLeft: string;
  icon: string;
  topBrawlers: Array<{
    name: string;
    winRate: number;
    useRate: number;
  }>;
}

const Home = () => {
  const [topBrawlers, setTopBrawlers] = useState<TopBrawler[]>([]);
  const [dailyMeta, setDailyMeta] = useState<DailyMetaEvent[]>([]);
  const [brawlerCount, setBrawlerCount] = useState<number>(0);

  useEffect(() => {
    const fetchData = async () => {
      // Mock daily meta
      const mockDailyMeta: DailyMetaEvent[] = [
        { mode: 'Solo Showdown', map: 'Acid Lakes', timeLeft: '10h 17m', icon: '🏟️', topBrawlers: [{ name: 'Edgar', winRate: 55, useRate: 0.7 },{ name: 'Shelly', winRate: 55, useRate: 1.0 },{ name: 'Leon', winRate: 55, useRate: 0.4 },{ name: 'Bull', winRate: 52, useRate: 0.5 },{ name: 'Rosa', winRate: 50, useRate: 6.7 }] },
        { mode: 'Brawl Ball', map: 'Spiraling Out', timeLeft: '22h 17m', icon: '⚽', topBrawlers: [{ name: 'Mortis', winRate: 73, useRate: 0.1 },{ name: 'Frank', winRate: 69, useRate: 0.1 },{ name: 'El Primo', winRate: 63, useRate: 0.1 },{ name: 'Darryl', winRate: 61, useRate: 0.1 },{ name: 'Bibi', winRate: 61, useRate: 0.6 }] },
        { mode: 'Gem Grab', map: 'On A Roll', timeLeft: '16h 17m', icon: '💎', topBrawlers: [{ name: 'Tara', winRate: 58, useRate: 0.3 },{ name: 'Gene', winRate: 57, useRate: 1.9 },{ name: 'Poco', winRate: 56, useRate: 3.1 },{ name: 'Pam', winRate: 55, useRate: 1.1 },{ name: 'Byron', winRate: 54, useRate: 1.2 }] },
      ];
      setDailyMeta(mockDailyMeta);

      try {
        const stats = await metaApi.getEnhancedStats();
        const formatted = stats.topBrawlers.slice(0, 6).map((b: any, i: number) => ({ id: b.id, name: b.name, winRate: b.winRate, pickRate: b.pickRate, trend: b.trend, rank: i + 1 }));
        setTopBrawlers(formatted);
      } catch {
        const fallback = (await metaApi.getMetaStats()).topBrawlers.slice(0, 6).map((b: any, i: number) => ({ id: b.id, name: b.name, winRate: b.winRate, pickRate: b.pickRate, rank: i + 1 }));
        setTopBrawlers(fallback);
      }

      try {
        const count = await metaApi.getBrawlerCount();
        setBrawlerCount(count);
      } catch (error) {
        console.error("Failed to fetch brawler count:", error);
        setBrawlerCount(-1); // Indicate an error
      }
    };
    fetchData();
  }, []);

  return (
    <div className="px-4 sm:px-0">
      {/* Hero */}
      <div className="text-center py-16 bg-gradient-to-r from-black/20 to-black/30 rounded-2xl mb-8">
        <h1 className="text-6xl font-bold text-white mb-4">🥊 <span className="text-transparent bg-clip-text bg-gradient-to-r from-yellow-400 to-orange-500">Brawl Buddy</span></h1>
        {brawlerCount !== -1 ? (
          <p className="text-white text-xl">Total Brawlers: {brawlerCount}</p>
        ) : (
          <p className="text-red-500 text-xl">Failed to load brawler count.</p>
        )}
      </div>
      {/* Daily Meta */}
      <section className="mb-8">
        <h2 className="text-2xl font-bold text-white mb-4">📊 Daily Meta</h2>
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {dailyMeta.map((e, idx) => (
            <div key={idx} className="bg-white/10 p-4 rounded-lg">
              <h3 className="text-white font-semibold">{e.map} ({e.mode})</h3>
            </div>
          ))}
        </div>
      </section>
      {/* Best Brawlers */}
      <section>
        <h2 className="text-2xl font-bold text-white mb-4">🏆 Top Brawlers</h2>
        <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-3 gap-4">
          {topBrawlers.map((b, i) => (
            <div key={i} className="bg-white/10 p-4 rounded-lg">
              <h3 className="text-white font-semibold">#{b.rank} {b.name}</h3>
            </div>
          ))}
        </div>
      </section>
    </div>
  );
};

export default Home;