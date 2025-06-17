// BattleLog types based on backend API models
export interface BattleLog {
  items: BattleLogItem[];
}

export interface BattleLogItem {
  battleTime: string;
  event: BattleEvent;
  battle: Battle;
}

export interface BattleEvent {
  id: number;
  mode: string;
  map: string;
}

export interface Battle {
  mode: string;
  type: string;
  result: string;
  duration: number;
  trophyChange: number;
  starTokensGained: number;
  teams: BattlePlayer[][];
  players?: BattlePlayer[]; // For solo modes
  starPlayer?: BattlePlayer; // Optional star player
}

export interface BattlePlayer {
  tag: string;
  name: string;
  brawler: BattleBrawler;
}

export interface BattleBrawler {
  id: number;
  name: string;
  power: number;
  trophies: number;
}