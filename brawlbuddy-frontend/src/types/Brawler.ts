// Brawler types based on backend API models
export interface StarPower {
  id: number;
  name: string;
  imageUrl?: string; // Example from Brawlify, adjust if API provides
  description?: string;
}

export interface Gadget {
  id: number;
  name: string;
  imageUrl?: string; // Example from Brawlify, adjust if API provides
  description?: string;
}

export interface BrawlerRarity {
  id?: number; // ID might not always be present or needed from all APIs
  name: string;
  color: string;
}

export interface BrawlerClass {
  id?: number; // ID might not always be present or needed
  name: string;
}

export interface Brawler {
  id: number;
  name: string;
  description?: string;
  imageUrl?: string; // Main image for the brawler
  imageUrl2?: string; // Secondary image or icon
  imageUrl3?: string; // Pin or other asset
  rarity: BrawlerRarity;
  class: BrawlerClass;
  starPowers: StarPower[];
  gadgets: Gadget[];
  // Gears can be added if the API supports it and it's in the C# model
  // gears?: Gear[]; 
}

// This is what the backend's /api/brawlers endpoint returns
export interface AllBrawlersResponse {
  brawlers: Brawler[];
  count: number;
}