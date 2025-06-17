import { useState, useEffect } from 'react';
import { useParams, Link } from 'react-router-dom';
import { brawlerApi } from '../services/api';
import { Brawler, StarPower, Gadget } from '../types/Brawler';

const BrawlerDetailPage = () => {
    const { id } = useParams<{ id: string }>();
    const [brawler, setBrawler] = useState<Brawler | null>(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState<string | null>(null);

    useEffect(() => {
        if (id) {
            const fetchBrawlerDetail = async () => {
                setLoading(true);
                setError(null);
                try {
                    const numericId = parseInt(id, 10);
                    if (isNaN(numericId)) {
                        throw new Error("Invalid Brawler ID");
                    }
                    const data = await brawlerApi.getBrawlerById(numericId);
                    setBrawler(data);
                } catch (err: any) {
                    setError(err.message || 'Failed to fetch brawler details');
                    setBrawler(null);
                } finally {
                    setLoading(false);
                }
            };
            fetchBrawlerDetail();
        }
    }, [id]);

    const getBrawlerImageUrl = (brawlerDetail: Brawler | null) => {
        if (brawlerDetail?.imageUrl) return brawlerDetail.imageUrl;
        // Fallback: Construct a URL or use a placeholder
        // Example: return `https://cdn.brawlify.com/brawlers-bs/${brawlerDetail?.id}.png`;
        return 'https://via.placeholder.com/128?text=' + encodeURIComponent(brawlerDetail?.name.substring(0,1) || '?');
    };
    
    const getAbilityImageUrl = (item: StarPower | Gadget) => {
        if (item.imageUrl) return item.imageUrl;
        return 'https://via.placeholder.com/48?text=?'; // Placeholder for ability icons
    };

    if (loading) {
        return (
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-yellow-400 mx-auto mb-4"></div>
                <p className="text-gray-300">Loading brawler details...</p>
            </div>
        );
    }

    if (error) {
        return (
            <div className="bg-red-900/50 border border-red-400 text-red-100 px-4 py-3 rounded mb-6 text-center">
                <strong className="font-bold">Error:</strong> {error}
                <div className="mt-4">
                    <Link to="/brawlers" className="px-4 py-2 bg-yellow-500 hover:bg-yellow-600 text-black font-semibold rounded-lg">
                        Back to Brawlers
                    </Link>
                </div>
            </div>
        );
    }

    if (!brawler) {
        return (
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-12 border border-white/20 text-center">
                <p className="text-gray-300">Brawler not found.</p>
                <div className="mt-4">
                    <Link to="/brawlers" className="px-4 py-2 bg-yellow-500 hover:bg-yellow-600 text-black font-semibold rounded-lg">
                        Back to Brawlers
                    </Link>
                </div>
            </div>
        );
    }
    
    const rarityColor = brawler.rarity?.color || '#FFFFFF';

    return (
        <div className="px-4 sm:px-0">
            <div className="bg-white/10 backdrop-blur-sm rounded-lg p-6 md:p-8 border border-white/20">
                {/* Header */}
                <div className="flex flex-col md:flex-row items-center mb-8">
                    <img
                        src={getBrawlerImageUrl(brawler)}
                        alt={brawler.name}
                        className="w-32 h-32 md:w-40 md:h-40 rounded-full object-contain bg-black/30 p-2 border-4"
                        style={{ borderColor: rarityColor }}
                        onError={(e) => (e.currentTarget.src = 'https://via.placeholder.com/128?text=?')}
                    />
                    <div className="md:ml-8 mt-4 md:mt-0 text-center md:text-left">
                        <h1 className="text-4xl md:text-5xl font-bold text-white" style={{ color: rarityColor }}>{brawler.name}</h1>
                        <p className="text-lg text-gray-300 mt-1">{brawler.class?.name || 'Unknown Class'}</p>
                        <p className="text-md mt-1" style={{ color: brawler.rarity?.color || '#ccc' }}>
                            {brawler.rarity?.name || 'Unknown Rarity'}
                        </p>
                    </div>
                </div>

                {/* Description */}
                {brawler.description && (
                    <div className="mb-8">
                        <h2 className="text-2xl font-semibold text-white mb-3">Description</h2>
                        <p className="text-gray-300 leading-relaxed">{brawler.description}</p>
                    </div>
                )}

                {/* Star Powers */}
                <div className="mb-8">
                    <h2 className="text-2xl font-semibold text-white mb-4">Star Powers ({brawler.starPowers.length})</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {brawler.starPowers.map(sp => (
                            <div key={sp.id} className="bg-white/5 p-4 rounded-lg border border-white/10 flex items-start space-x-3">
                                <img src={getAbilityImageUrl(sp)} alt={sp.name} className="w-12 h-12 object-contain rounded bg-black/20"/>
                                <div>
                                    <h3 className="font-semibold text-yellow-400">{sp.name}</h3>
                                    <p className="text-sm text-gray-400">{sp.description || "No description available."}</p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>

                {/* Gadgets */}
                <div>
                    <h2 className="text-2xl font-semibold text-white mb-4">Gadgets ({brawler.gadgets.length})</h2>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {brawler.gadgets.map(gadget => (
                            <div key={gadget.id} className="bg-white/5 p-4 rounded-lg border border-white/10 flex items-start space-x-3">
                                 <img src={getAbilityImageUrl(gadget)} alt={gadget.name} className="w-12 h-12 object-contain rounded bg-black/20"/>
                                <div>
                                    <h3 className="font-semibold text-green-400">{gadget.name}</h3>
                                    <p className="text-sm text-gray-400">{gadget.description || "No description available."}</p>
                                </div>
                            </div>
                        ))}
                    </div>
                </div>
                
                <div className="mt-8 text-center">
                    <Link to="/brawlers" className="px-6 py-3 bg-yellow-500 hover:bg-yellow-600 text-black font-semibold rounded-lg transition-colors">
                        ← Back to All Brawlers
                    </Link>
                </div>
            </div>
        </div>
    );
};

export default BrawlerDetailPage;
