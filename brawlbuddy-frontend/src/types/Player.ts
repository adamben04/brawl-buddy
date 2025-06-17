// Player types based on backend API models
export interface Player {
  tag: string;
  name: string;
  nameColor: string;
  icon: PlayerIcon;
  trophies: number;
  highestTrophies: number;
  expLevel: number;
  expPoints: number;
  isQualifiedFromChampionshipChallenge: boolean;
  soloVictories: number;
  duoVictories: number;
  bestRoboRumbleTime: number;
  bestTimeAsBigBrawler: number;
  club?: PlayerClub;
  brawlers: PlayerBrawler[];
}

export interface PlayerIcon {
  id: number;
}

export interface PlayerClub {
  tag: string;
  name: string;
  role?: string; // Optional since backend might not have this
}

export interface PlayerBrawler {
  id: number;
  name: string;
  power: number;
  rank: number;
  trophies: number;
  highestTrophies: number;
  gadgets: PlayerGadget[];
  starPowers: PlayerStarPower[];
  gears: PlayerGear[];
}

export interface PlayerGadget {
  id: number;
  name: string;
}

export interface PlayerStarPower {
  id: number;
  name: string;
}

export interface PlayerGear {
  id: number;
  name: string;
  level: number;
}