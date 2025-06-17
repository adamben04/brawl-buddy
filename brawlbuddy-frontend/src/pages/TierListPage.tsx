import { useState, useEffect } from 'react';
import { metaApi, brawlerApi } from '../services/api';
import { Brawler } from '../types/Brawler';

interface TierData {
    tier: string;
    brawlers: Brawler[];
    color: string;
    description: string;
}

const TierListPage = () => {
    const [tierLists, setTierLists] = useState<any>(null);
    const [selectedMode, setSelectedMode] = useState<string>('Overall');
    const [brawlers, setBrawlers] = useState<Brawler[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    const gameModes = [
        'Overall', 'Gem Grab', 'Brawl Ball', 'Heist', 'Bounty', 
        'Siege', 'Hot Zone', 'Knockout', 'Showdown'
    ];

    const tierColors = {
        'S': 'from-red-500 to-red-600',
        'A': 'from-orange-500 to-orange-600',
        'B': 'from-yellow-500 to-yellow-600',
        'C': 'from-green-500 to-green-600',
        'D': 'from-blue-500 to-blue-600'
    };

    useEffect(() => {
        fetchData();
    }, []);

    const fetchData = async () => {
        setLoading(true);
        setError(null);
        try {        const [tierData, brawlerData] = await Promise.all([
            metaApi.getTierList(),
            brawlerApi.getAllBrawlers()
        ]);        setTierLists(tierData);
        setBrawlers(brawlerData.brawlers || []);
        } catch (err: any) {
            setError(err.message || 'Failed to fetch tier list data');
        } finally {
            setLoading(false);
        }
    };

    const getCurrentTierList = (): TierData[] => {
        if (!tierLists || !brawlers.length) return [];

        const currentList = tierLists[selectedMode] || tierLists['Overall'];
        if (!currentList) return [];

        const tierMap: { [key: string]: Brawler[] } = {
            'S': [], 'A': [], 'B': [], 'C': [], 'D': []
        };

        // Group brawlers by tier
        currentList.forEach((tierBrawler: any) => {
            const brawler = brawlers.find(b => b.id === tierBrawler.brawlerId);
            if (brawler && tierMap[tierBrawler.tier]) {
                tierMap[tierBrawler.tier].push(brawler);
            }
        });

        return [
            { tier: 'S', brawlers: tierMap['S'], color: tierColors['S'], description: 'Meta defining - Dominant in current meta' },
            { tier: 'A', brawlers: tierMap['A'], color: tierColors['A'], description: 'Very Strong - Excellent picks for most situations' },
            { tier: 'B', brawlers: tierMap['B'], color: tierColors['B'], description: 'Good - Solid choices with clear strengths' },
            { tier: 'C', brawlers: tierMap['C'], color: tierColors['C'], description: 'Average - Situational picks, needs right conditions' },
            { tier: 'D', brawlers: tierMap['D'], color: tierColors['D'], description: 'Below Average - Struggles in current meta' }
        ].filter(tier => tier.brawlers.length > 0);
    };

    const getBrawlerRarity = (rarity: string) => {
        const rarityColors = {
            'Trophy Road': 'text-blue-400',
            'Rare': 'text-green-400',
            'Super Rare': 'text-blue-500',
            'Epic': 'text-purple-400',
            'Mythic': 'text-pink-400',
            'Legendary': 'text-yellow-400'
        };
        return rarityColors[rarity as keyof typeof rarityColors] || 'text-gray-400';
    };

    return (
        <div className="px-4 sm:px-0">
            {/* Header */}
            <div className="text-center mb-8">
                <h1 className="text-4xl font-bold text-white mb-4">
                    📊 <span className="bg-gradient-to-r from-yellow-400 to-orange-500 bg-clip-text text-transparent">
                        Tier Lists
                    </span>
                </h1>
                <p className="text-xl text-gray-300 max-w-2xl mx-auto">
                    Discover the strongest brawlers in the current meta across all game modes
                </p>
            </div>

            {/* Mode Selection */}
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 mb-6">
                <h2 className="text-lg font-semibold text-white mb-4">🎮 Select Game Mode</h2>
                <div className="flex flex-wrap gap-2">
                    {gameModes.map((mode) => (
                        <button
                            key={mode}
                            onClick={() => setSelectedMode(mode)}
                            className={`px-4 py-2 rounded-lg font-medium transition-all ${
                                selectedMode === mode
                                    ? 'bg-yellow-500 text-black'
                                    : 'bg-white/10 text-gray-300 hover:bg-white/20'
                            }`}
                        >
                            {mode}
                        </button>
                    ))}
                </div>
            </div>

            {/* Error State */}
            {error && (
                <div className="bg-red-900/50 border border-red-400 text-red-100 px-4 py-3 rounded mb-6">
                    <strong className="font-bold">Error:</strong> {error}
                </div>
            )}

            {/* Loading State */}
            {loading && (
                <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
                    <p className="text-gray-300">Loading tier lists...</p>
                </div>
            )}

            {/* Tier Lists */}
            {!loading && !error && (
                <div className="space-y-6">
                    <div className="text-center mb-6">
                        <h2 className="text-2xl font-bold text-white">
                            {selectedMode} Tier List
                        </h2>
                        <p className="text-gray-400 mt-2">
                            Rankings based on current meta analysis and win rates
                        </p>
                    </div>

                    {getCurrentTierList().map((tierData) => (
                        <div key={tierData.tier} className="bg-white/10 backdrop-blur-sm rounded-lg border border-white/20 overflow-hidden">
                            {/* Tier Header */}
                            <div className={`bg-gradient-to-r ${tierData.color} p-4`}>
                                <div className="flex items-center justify-between">
                                    <div className="flex items-center">
                                        <div className="text-3xl font-bold text-white mr-4">
                                            {tierData.tier}
                                        </div>
                                        <div>
                                            <div className="text-white font-semibold">
                                                {tierData.description.split(' - ')[0]}
                                            </div>
                                            <div className="text-white/80 text-sm">
                                                {tierData.description.split(' - ')[1]}
                                            </div>
                                        </div>
                                    </div>
                                    <div className="text-white/90 text-sm">
                                        {tierData.brawlers.length} brawler{tierData.brawlers.length !== 1 ? 's' : ''}
                                    </div>
                                </div>
                            </div>

                            {/* Brawlers Grid */}
                            <div className="p-6">
                                <div className="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-4 xl:grid-cols-6 gap-4">
                                    {tierData.brawlers.map((brawler) => (
                                        <div
                                            key={brawler.id}
                                            className="bg-white/10 rounded-lg p-4 border border-white/10 hover:bg-white/20 transition-colors group"
                                        >
                                            <div className="text-center">
                                                <div className="text-2xl mb-2 group-hover:scale-110 transition-transform">
                                                    🥊
                                                </div>
                                                <h4 className="font-semibold text-white text-sm mb-1">
                                                    {brawler.name}
                                                </h4>                                                <div className={`text-xs ${getBrawlerRarity(brawler.rarity?.name || 'Unknown')}`}>
                                                    {brawler.rarity?.name || 'Unknown'}
                                                </div>
                                                <div className="text-xs text-gray-400 mt-1">
                                                    {brawler.class?.name || 'Fighter'}
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                        </div>
                    ))}

                    {getCurrentTierList().length === 0 && (
                        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                            <div className="text-6xl mb-4">📊</div>
                            <h3 className="text-xl font-semibold text-white mb-2">No Tier Data Available</h3>
                            <p className="text-gray-300">
                                Tier list data for {selectedMode} is not available yet.
                            </p>
                        </div>
                    )}

                    {/* Legend */}
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
                        <h3 className="text-lg font-semibold text-white mb-4">📖 Tier Explanations</h3>
                        <div className="grid md:grid-cols-2 lg:grid-cols-3 gap-4">
                            {Object.entries(tierColors).map(([tier, color]) => (
                                <div key={tier} className="flex items-center">
                                    <div className={`w-8 h-8 rounded bg-gradient-to-r ${color} mr-3 flex items-center justify-center text-white font-bold`}>
                                        {tier}
                                    </div>
                                    <div className="text-sm text-gray-300">
                                        {tier === 'S' && 'Meta defining picks'}
                                        {tier === 'A' && 'Very strong choices'}
                                        {tier === 'B' && 'Good, solid picks'}
                                        {tier === 'C' && 'Average, situational'}
                                        {tier === 'D' && 'Below average'}
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default TierListPage;