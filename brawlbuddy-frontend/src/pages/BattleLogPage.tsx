import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { playerApi } from '../services/api';
import { BattleLogItem } from '../types/BattleLog';

const BattleLogPage = () => {
    const { tag } = useParams<{ tag: string }>();
    const navigate = useNavigate();
    const [battles, setBattles] = useState<BattleLogItem[]>([]);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [searchTag, setSearchTag] = useState(tag || '');

    useEffect(() => {
        if (tag) {
            fetchBattleLog(tag);
        }
    }, [tag]);

    const fetchBattleLog = async (playerTag: string) => {
        setLoading(true);
        setError(null);
        try {
            const battleData = await playerApi.getBattleLog(playerTag);
            setBattles(battleData.items || []);
        } catch (err: any) {
            setError(err.message || 'Failed to fetch battle log');
            setBattles([]);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = () => {
        if (searchTag.trim()) {
            navigate(`/battles/${encodeURIComponent(searchTag.trim())}`);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') {
            handleSearch();
        }
    };

    const formatTime = (timestamp: string) => {
        const date = new Date(timestamp);
        const now = new Date();
        const diffMs = now.getTime() - date.getTime();
        const diffHours = Math.floor(diffMs / (1000 * 60 * 60));
        const diffDays = Math.floor(diffHours / 24);

        if (diffDays > 0) {
            return `${diffDays} day${diffDays > 1 ? 's' : ''} ago`;
        } else if (diffHours > 0) {
            return `${diffHours} hour${diffHours > 1 ? 's' : ''} ago`;
        } else {
            const diffMinutes = Math.floor(diffMs / (1000 * 60));
            return `${diffMinutes} minute${diffMinutes > 1 ? 's' : ''} ago`;
        }
    };

    const getResultColor = (result: string) => {
        switch (result.toLowerCase()) {
            case 'victory': return 'text-green-400';
            case 'defeat': return 'text-red-400';
            case 'draw': return 'text-yellow-400';
            default: return 'text-gray-400';
        }
    };

    const getResultIcon = (result: string) => {
        switch (result.toLowerCase()) {
            case 'victory': return '🏆';
            case 'defeat': return '💀';
            case 'draw': return '🤝';
            default: return '⚔️';
        }
    };

    const getModeIcon = (mode: string) => {
        const modeIcons: { [key: string]: string } = {
            'Gem Grab': '💎',
            'Brawl Ball': '⚽',
            'Heist': '💰',
            'Bounty': '🎯',
            'Siege': '🏰',
            'Hot Zone': '🔥',
            'Knockout': '👊',
            'Solo Showdown': '👤',
            'Duo Showdown': '👥',
            'Big Game': '🐻',
            'Robo Rumble': '🤖'
        };
        return modeIcons[mode] || '⚔️';
    };

    return (
        <div className="px-4 sm:px-0">
            {/* Search Section */}
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 mb-6">
                <h1 className="text-2xl font-bold text-white mb-4">⚔️ Battle Log</h1>
                <div className="flex flex-col sm:flex-row gap-4">
                    <input
                        type="text"
                        value={searchTag}
                        onChange={(e) => setSearchTag(e.target.value)}
                        onKeyPress={handleKeyPress}
                        placeholder="Enter player tag (e.g., #2PP, #YYYYYYY)"
                        className="flex-1 px-4 py-3 bg-white/10 border border-white/20 rounded-lg text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-yellow-400 focus:border-transparent"
                    />
                    <button
                        onClick={handleSearch}
                        disabled={loading}
                        className="px-6 py-3 bg-purple-500 hover:bg-purple-600 disabled:bg-gray-500 text-white font-semibold rounded-lg transition-colors"
                    >
                        {loading ? 'Loading...' : 'View Battles'}
                    </button>
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
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-purple-400 mx-auto mb-4"></div>
                    <p className="text-gray-300">Loading battle log...</p>
                </div>
            )}

            {/* Battle Log */}
            {battles.length > 0 && !loading && (
                <div className="space-y-4">
                    <div className="text-center mb-6">
                        <h2 className="text-xl font-bold text-white">Recent Battles</h2>
                        <p className="text-gray-400">Last {battles.length} battles</p>
                    </div>

                    {battles.map((battle, index) => (
                        <div key={index} className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
                            {/* Battle Header */}
                            <div className="flex items-center justify-between mb-4">
                                <div className="flex items-center space-x-4">
                                    <div className="text-2xl">
                                        {getModeIcon(battle.battle.mode)}
                                    </div>
                                    <div>
                                        <h3 className="font-semibold text-white">{battle.battle.mode}</h3>
                                        <p className="text-sm text-gray-400">{battle.event.map}</p>
                                    </div>
                                </div>
                                <div className="flex items-center space-x-4">
                                    <div className={`text-lg font-bold ${getResultColor(battle.battle.result)}`}>
                                        {getResultIcon(battle.battle.result)} {battle.battle.result}
                                    </div>
                                    <div className="text-sm text-gray-400">
                                        {formatTime(battle.battleTime)}
                                    </div>
                                </div>
                            </div>

                            {/* Battle Details */}
                            <div className="grid md:grid-cols-2 gap-6">
                                {/* Players */}
                                <div>
                                    <h4 className="font-semibold text-white mb-3">
                                        {battle.battle.teams ? 'Your Team' : 'Players'}
                                    </h4>
                                    <div className="space-y-2">
                                        {battle.battle.teams ? (
                                            // Team-based modes
                                            battle.battle.teams[0].map((player, playerIndex) => (
                                                <div key={playerIndex} className="flex items-center justify-between bg-white/10 rounded p-3">
                                                    <div className="flex items-center space-x-3">
                                                        <div className="text-lg">🥊</div>
                                                        <div>
                                                            <div className="font-medium text-white">{player.name}</div>
                                                            <div className="text-sm text-gray-400">{player.brawler?.name}</div>
                                                        </div>
                                                    </div>
                                                    <div className="text-sm">
                                                        <div className="text-yellow-400">
                                                            {player.brawler?.trophies} 🏆
                                                        </div>
                                                        <div className="text-gray-400">
                                                            Power {player.brawler?.power}
                                                        </div>
                                                    </div>
                                                </div>
                                            ))
                                        ) : (
                                            // Solo/Duo Showdown
                                            battle.battle.players?.slice(0, 5).map((player, playerIndex) => (
                                                <div key={playerIndex} className="flex items-center justify-between bg-white/10 rounded p-3">
                                                    <div className="flex items-center space-x-3">
                                                        <div className="text-lg">
                                                            {playerIndex === 0 ? '🥇' : playerIndex === 1 ? '🥈' : playerIndex === 2 ? '🥉' : '🏅'}
                                                        </div>
                                                        <div>
                                                            <div className="font-medium text-white">{player.name}</div>
                                                            <div className="text-sm text-gray-400">{player.brawler?.name}</div>
                                                        </div>
                                                    </div>
                                                    <div className="text-sm">
                                                        <div className="text-yellow-400">
                                                            {player.brawler?.trophies} 🏆
                                                        </div>
                                                        <div className="text-gray-400">
                                                            Power {player.brawler?.power}
                                                        </div>
                                                    </div>
                                                </div>
                                            ))
                                        )}
                                    </div>
                                </div>

                                {/* Trophy Change & Stats */}
                                <div>
                                    <h4 className="font-semibold text-white mb-3">Battle Stats</h4>
                                    <div className="space-y-3">
                                        <div className="bg-white/10 rounded p-3">
                                            <div className="flex justify-between items-center">
                                                <span className="text-gray-300">Trophy Change</span>
                                                <span className={`font-bold ${
                                                    battle.battle.trophyChange > 0 ? 'text-green-400' : 
                                                    battle.battle.trophyChange < 0 ? 'text-red-400' : 'text-gray-400'
                                                }`}>
                                                    {battle.battle.trophyChange > 0 ? '+' : ''}{battle.battle.trophyChange}
                                                </span>
                                            </div>
                                        </div>

                                        <div className="bg-white/10 rounded p-3">
                                            <div className="flex justify-between items-center">
                                                <span className="text-gray-300">Duration</span>
                                                <span className="text-white">
                                                    {Math.floor(battle.battle.duration / 60)}m {battle.battle.duration % 60}s
                                                </span>
                                            </div>
                                        </div>

                                        {battle.battle.starPlayer && (
                                            <div className="bg-white/10 rounded p-3">
                                                <div className="flex justify-between items-center">
                                                    <span className="text-gray-300">Star Player</span>
                                                    <span className="text-yellow-400">
                                                        ⭐ {battle.battle.starPlayer.name}
                                                    </span>
                                                </div>
                                            </div>
                                        )}

                                        <div className="bg-white/10 rounded p-3">
                                            <div className="flex justify-between items-center">
                                                <span className="text-gray-300">Battle Type</span>
                                                <span className="text-white capitalize">
                                                    {battle.battle.type?.replace(/([A-Z])/g, ' $1').trim()}
                                                </span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            )}

            {/* Empty State */}
            {battles.length === 0 && !loading && !error && (
                <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                    <div className="text-6xl mb-4">⚔️</div>
                    <h3 className="text-xl font-semibold text-white mb-2">No Battle Log Found</h3>
                    <p className="text-gray-300">
                        Enter a player tag above to view their recent battle history.
                    </p>
                </div>
            )}

            {/* Back to Player Link */}
            {tag && (
                <div className="text-center mt-6">
                    <button
                        onClick={() => navigate(`/player/${encodeURIComponent(tag)}`)}
                        className="px-6 py-3 bg-blue-500 hover:bg-blue-600 text-white font-semibold rounded-lg transition-colors"
                    >
                        ← Back to Player Profile
                    </button>
                </div>
            )}
        </div>
    );
};

export default BattleLogPage;