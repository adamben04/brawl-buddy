// API service to connect frontend with backend
import axios from 'axios';
import { Player } from '../types/Player';
import { BattleLog } from '../types/BattleLog';
import { Brawler } from '../types/Brawler';

const api = axios.create({
  baseURL: 'http://localhost:5000/api',
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
  getAllBrawlers: async (): Promise<{ brawlers: Brawler[]; count: number }> => {
    const response = await api.get('/brawler');
    return response.data as { brawlers: Brawler[]; count: number };
  },

  // Get specific brawler by ID
  getBrawler: async (id: number): Promise<Brawler> => {
    const response = await api.get(`/brawler/${id}`);
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
};

export default api;