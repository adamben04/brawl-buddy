// API service to connect frontend with backend
import axios from 'axios';
import { Player } from '../types/Player';
import { BattleLog } from '../types/BattleLog';
import { Brawler, AllBrawlersResponse } from '../types/Brawler';

const api = axios.create({
  baseURL: 'http://localhost:5001/api',
  headers: {
    'Content-Type': 'application/json',
  },
  timeout: 10000,
});

// Add response interceptor for error handling
api.interceptors.response.use(
  (response) => response,
  (error) => {
    console.error('API Error:', error.response?.data || error.message);
    return Promise.reject(error);
  }
);

// Player API calls
export const playerApi = {
  // Test backend connection
  test: async (): Promise<{ message: string; timestamp: string }> => {
    const response = await api.get('/player/test');
    return response.data as { message: string; timestamp: string };
  },

  // Get player profile by tag
  getPlayer: async (playerTag: string): Promise<Player> => {
    const response = await api.get(`/player/${encodeURIComponent(playerTag)}`);
    return response.data as Player;
  },
  // Get player battle log
  getBattleLog: async (playerTag: string): Promise<BattleLog> => {
    const response = await api.get(`/player/${encodeURIComponent(playerTag)}/battles`);
    return response.data as BattleLog;
  },
};

// Brawler API calls
export const brawlerApi = {
  // Get all brawlers
  getAllBrawlers: async (): Promise<AllBrawlersResponse> => {
    const response = await api.get('/brawler'); // Matches BrawlerController route
    return response.data as AllBrawlersResponse;
  },

  // Get a specific brawler by ID
  getBrawlerById: async (id: number): Promise<Brawler> => {
    const response = await api.get(`/brawler/${id}`); // Matches BrawlerController route
    return response.data as Brawler;
  },
};

// Meta API calls
export const metaApi = {
  // Get tier list by mode
  getTierList: async (mode?: string): Promise<any> => {
    const params = mode ? `?mode=${encodeURIComponent(mode)}` : '';
    const response = await api.get(`/meta/tiers${params}`);
    return response.data;
  },

  // Get meta statistics
  getMetaStats: async (): Promise<any> => {
    const response = await api.get('/meta/stats');
    return response.data;
  },

  // Get current events
  getEvents: async (): Promise<any> => {
    const response = await api.get('/events');
    return response.data;
  },

  // Get enhanced stats combining multiple data sources
  getEnhancedStats: async (): Promise<any> => {
    const response = await api.get('/meta/enhanced-stats');
    return response.data;
  },

  // Get real win rates
  getWinRates: async (mode?: string): Promise<any> => {
    const params = mode ? `?mode=${encodeURIComponent(mode)}` : '';
    const response = await api.get(`/meta/winrates${params}`);
    return response.data;
  },

  // Get real pick rates
  getPickRates: async (mode?: string): Promise<any> => {
    const params = mode ? `?mode=${encodeURIComponent(mode)}` : '';
    const response = await api.get(`/meta/pickrates${params}`);
    return response.data;
  },  // Get total unique brawler count
  getBrawlerCount: async (): Promise<number> => {
    const response = await api.get('/meta/brawlers/count');
    const data = response.data as { count: number };
    return data.count; // Extract the count from the response object
  }
};

export default api;