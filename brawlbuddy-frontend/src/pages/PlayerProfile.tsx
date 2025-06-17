import { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { playerApi } from '../services/api';
import { Player } from '../types/Player';

const PlayerProfile = () => {
    const { tag } = useParams<{ tag: string }>();
    const navigate = useNavigate();
    const [player, setPlayer] = useState<Player | null>(null);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState<string | null>(null);
    const [searchTag, setSearchTag] = useState(tag || '');

    useEffect(() => {
        if (tag) {
            fetchPlayer(tag);
        }
    }, [tag]);

    const fetchPlayer = async (playerTag: string) => {
        setLoading(true);
        setError(null);
        try {
            const playerData = await playerApi.getPlayer(playerTag);
            setPlayer(playerData);
        } catch (err: any) {
            setError(err.message || 'Failed to fetch player data');
            setPlayer(null);
        } finally {
            setLoading(false);
        }
    };

    const handleSearch = () => {
        if (searchTag.trim()) {
            navigate(`/player/${encodeURIComponent(searchTag.trim())}`);
        }
    };

    const handleKeyPress = (e: React.KeyboardEvent) => {
        if (e.key === 'Enter') {
            handleSearch();
        }
    };

    return (
        <div className="px-4 sm:px-0">
            {/* Search Section */}
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20 mb-6">
                <h1 className="text-2xl font-bold text-white mb-4">🔍 Player Search</h1>
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
                        className="px-6 py-3 bg-yellow-500 hover:bg-yellow-600 disabled:bg-gray-500 text-black font-semibold rounded-lg transition-colors"
                    >
                        {loading ? 'Searching...' : 'Search Player'}
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
                    <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
                    <p className="text-gray-300">Loading player data...</p>
                </div>
            )}

            {/* Player Data */}
            {player && !loading && (
                <div className="space-y-6">
                    {/* Player Header */}
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
                        <div className="flex items-center justify-between mb-4">
                            <div>
                                <h2 className="text-3xl font-bold text-white">{player.name}</h2>
                                <p className="text-gray-300">{player.tag}</p>
                            </div>
                            <div className="text-right">
                                <div className="text-2xl font-bold text-yellow-400">{player.trophies}</div>
                                <div className="text-sm text-gray-300">Current Trophies</div>
                            </div>
                        </div>
                        
                        <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                            <div className="text-center">
                                <div className="text-xl font-bold text-orange-400">{player.highestTrophies}</div>
                                <div className="text-sm text-gray-300">Highest Trophies</div>
                            </div>
                            <div className="text-center">
                                <div className="text-xl font-bold text-purple-400">{player.expLevel}</div>
                                <div className="text-sm text-gray-300">Experience Level</div>
                            </div>
                            <div className="text-center">
                                <div className="text-xl font-bold text-green-400">{player.soloVictories}</div>
                                <div className="text-sm text-gray-300">Solo Victories</div>
                            </div>
                            <div className="text-center">
                                <div className="text-xl font-bold text-blue-400">{player.duoVictories}</div>
                                <div className="text-sm text-gray-300">Duo Victories</div>
                            </div>
                        </div>
                    </div>

                    {/* Brawlers Section */}
                    <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
                        <h3 className="text-xl font-bold text-white mb-4">🥊 Brawlers ({player.brawlers.length})</h3>
                        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                            {player.brawlers
                                .sort((a, b) => b.trophies - a.trophies)
                                .slice(0, 12)
                                .map((brawler) => (
                                <div key={brawler.id} className="bg-white/10 rounded-lg p-4 border border-white/10">
                                    <div className="flex items-center justify-between mb-2">
                                        <h4 className="font-semibold text-white">{brawler.name}</h4>
                                        <span className="text-sm text-yellow-400">Level {brawler.power}</span>
                                    </div>
                                    <div className="flex justify-between text-sm">
                                        <div>
                                            <div className="text-yellow-400">{brawler.trophies} 🏆</div>
                                            <div className="text-gray-400">Current</div>
                                        </div>
                                        <div>
                                            <div className="text-orange-400">{brawler.highestTrophies} 🏆</div>
                                            <div className="text-gray-400">Highest</div>
                                        </div>
                                        <div>
                                            <div className="text-blue-400">{brawler.rank}</div>
                                            <div className="text-gray-400">Rank</div>
                                        </div>
                                    </div>
                                </div>
                            ))}
                        </div>
                        
                        {player.brawlers.length > 12 && (
                            <div className="text-center mt-4">
                                <p className="text-gray-400">
                                    Showing top 12 brawlers. Total: {player.brawlers.length}
                                </p>
                            </div>
                        )}
                    </div>

                    {/* Club Info (if available) */}
                    {player.club && (
                        <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 border border-white/20">
                            <h3 className="text-xl font-bold text-white mb-4">🏛️ Club</h3>
                            <div className="flex items-center justify-between">
                                <div>
                                    <h4 className="font-semibold text-white">{player.club.name}</h4>
                                    <p className="text-gray-300">{player.club.tag}</p>
                                </div>                                <div className="text-right">
                                    <div className="text-blue-400 font-bold">{player.club.role || 'Member'}</div>
                                    <div className="text-sm text-gray-400">Role</div>
                                </div>
                            </div>
                        </div>
                    )}

                    {/* Battle Log Link */}
                    <div className="text-center">
                        <button
                            onClick={() => navigate(`/battles/${encodeURIComponent(player.tag)}`)}
                            className="px-6 py-3 bg-purple-500 hover:bg-purple-600 text-white font-semibold rounded-lg transition-colors"
                        >
                            View Battle Log ⚔️
                        </button>
                    </div>
                </div>
            )}

            {/* Empty State */}
            {!player && !loading && !error && (
                <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                    <div className="text-6xl mb-4">🔍</div>
                    <h3 className="text-xl font-semibold text-white mb-2">Search for a Player</h3>
                    <p className="text-gray-300">
                        Enter a player tag above to view detailed statistics and brawler collection.
                    </p>
                </div>
            )}
        </div>
    );
};

export default PlayerProfile;