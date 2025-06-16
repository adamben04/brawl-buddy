// Brawler types based on backend API models
export interface Brawler {
  id: number;
  name: string;
  starPowers: BrawlerStarPower[];
  gadgets: BrawlerGadget[];
}

export interface BrawlerStarPower {
  id: number;
  name: string;
}

export interface BrawlerGadget {
  id: number;
  name: string;
}