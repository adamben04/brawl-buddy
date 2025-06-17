import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';

// --- Interfaces for Brawlify API Responses ---

// Interface for Brawler Star Power
interface BrawlifyStarPower {
  id: number;
  name: string;
  path: string;
  version: number;
  description: string;
  descriptionHtml: string;
  imageUrl: string;
  released: boolean;
}

// Interface for Brawler Gadget
interface BrawlifyGadget {
  id: number;
  name: string;
  path: string;
  version: number;
  description: string;
  descriptionHtml: string;
  imageUrl: string;
  released: boolean;
}

// Interface for Brawler Class
interface BrawlifyBrawlerClass {
  id: number;
  name: string;
}

// Interface for Brawler Rarity
interface BrawlifyBrawlerRarity {
  id: number;
  name: string;
  color: string;
}

// Interface for a single Brawler from Brawlify API
interface BrawlifyBrawler {
  id: number;
  avatarId: number;
  name: string;
  hash: string;
  path: string;
  fankit: string;
  released: boolean;
  version: number;
  link: string;
  imageUrl: string;
  imageUrl2: string;
  imageUrl3: string;
  class: BrawlifyBrawlerClass;
  rarity: BrawlifyBrawlerRarity;
  unlock: number | null;
  description: string;
  descriptionHtml: string;
  starPowers: BrawlifyStarPower[];
  gadgets: BrawlifyGadget[];
  videos: any[]; // Define more specifically if needed
}

// Interface for the /brawlers endpoint response
interface BrawlifyBrawlersResponse {
  list: BrawlifyBrawler[];
}

// Interface for Map Environment
interface BrawlifyMapEnvironment {
  id: number;
  name: string;
  hash: string;
  path: string;
  version: number;
  imageUrl: string;
}

// Interface for Game Mode (nested in Map and Events)
interface BrawlifyGameModeInfo {
  id: number;
  name: string;
  hash: string;
  version: number;
  color: string;
  bgColor: string;
  link: string;
  imageUrl: string;
  scId?: number; // Present in Event's gameMode
}

// Interface for a single Map from Brawlify API
interface BrawlifyMap {
  id: number;
  new: boolean;
  disabled: boolean;
  name: string;
  hash: string;
  version: number;
  link: string;
  imageUrl: string;
  credit: string | null;
  environment: BrawlifyMapEnvironment;
  gameMode: BrawlifyGameModeInfo;
  lastActive: number | null;
  dataUpdated: number;
  stats?: BrawlifyEventStat[]; // Present in Event's map
}

// Interface for the /maps endpoint response
interface BrawlifyMapsResponse {
  list: BrawlifyMap[];
}

// Interface for a single Game Mode from Brawlify API
interface BrawlifyGameMode extends BrawlifyGameModeInfo {
  scId: number;
  scHash: string;
  disabled: boolean;
  title: string;
  tutorial: string;
  description: string;
  shortDescription: string;
  sort1: number;
  sort2: number;
  imageUrl2: string;
  lastActive: number | null;
  TID: string;
}

// Interface for the /gamemodes endpoint response
interface BrawlifyGameModesResponse {
  list: BrawlifyGameMode[];
}

// Interface for Player Icons
interface BrawlifyPlayerIcon {
  id: number;
  name: string;
  name2: string;
  imageUrl: string;
  brawler: number | null;
  requiredTotalTrophies: number;
  sortOrder: number;
  isReward: boolean;
  isAvailableForOffers: boolean;
}

// Interface for Club Icons
interface BrawlifyClubIcon {
  id: number;
  imageUrl: string;
}

// Interface for the /icons endpoint response
interface BrawlifyIconsResponse {
  player: { [key: string]: BrawlifyPlayerIcon };
  club: { [key: string]: BrawlifyClubIcon };
}

// Interface for Event Slot
interface BrawlifyEventSlot {
  id: number;
  name: string;
  emoji: string;
  hash: string;
  listAlone: boolean;
  hideable: boolean;
  hideForSlot: number | null;
  background: string | null;
}

// Interface for Event Stats
interface BrawlifyEventStat {
  brawler: number;
  winRate: number;
  useRate: number;
}

// Interface for an active/upcoming Event
interface BrawlifyEvent {
  slot: BrawlifyEventSlot;
  predicted: boolean;
  startTime: string;
  endTime: string;
  reward: number;
  map: BrawlifyMap; // Reuses the Map interface, which might include stats
  modifier: any | null; // Define more specifically if needed
  historyLength?: number; // For upcoming events
}

// Interface for the /events endpoint response
interface BrawlifyEventsResponse {
  active: BrawlifyEvent[];
  upcoming: BrawlifyEvent[];
}


@Injectable({
  providedIn: 'root'
})
export class BrawlifyService {

  private apiUrl = 'https://api.brawlify.com/v1'; // Base URL for the Brawlify API
  // No API key is needed for Brawlify API as per their documentation.
  // Instead, a User-Agent header is requested.
  private appUserAgent = 'BrawlBuddy/1.0 (YourAppName; YourContactInfo)'; // Customize this

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    return new HttpHeaders({
      'User-Agent': this.appUserAgent
    });
  }

  /**
   * Retrieves events from the Brawlify API.
   * @returns An Observable containing the API response.
   */
  getEvents(): Observable<BrawlifyEventsResponse> {
    const url = `${this.apiUrl}/events`;
    const headers = this.getHeaders();

    return this.http.get<BrawlifyEventsResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Retrieves a list of brawlers from the Brawlify API.
   * @returns An Observable containing the API response.
   */
  getBrawlers(): Observable<BrawlifyBrawlersResponse> {
    const url = `${this.apiUrl}/brawlers`;
    const headers = this.getHeaders();

    return this.http.get<BrawlifyBrawlersResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Retrieves a list of maps from the Brawlify API.
   * @returns An Observable containing the API response.
   */
  getMaps(): Observable<BrawlifyMapsResponse> {
    const url = `${this.apiUrl}/maps`;
    const headers = this.getHeaders();

    return this.http.get<BrawlifyMapsResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Retrieves a list of game modes from the Brawlify API.
   * @returns An Observable containing the API response.
   */
  getGameModes(): Observable<BrawlifyGameModesResponse> {
    const url = `${this.apiUrl}/gamemodes`;
    const headers = this.getHeaders();

    return this.http.get<BrawlifyGameModesResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Retrieves icons from the Brawlify API.
   * @returns An Observable containing the API response.
   */
  getIcons(): Observable<BrawlifyIconsResponse> {
    const url = `${this.apiUrl}/icons`;
    const headers = this.getHeaders();

    return this.http.get<BrawlifyIconsResponse>(url, { headers }).pipe(
      catchError(this.handleError)
    );
  }

  /**
   * Handles HTTP errors.
   * @param error The error object.
   * @returns An Observable that emits an error.
   */
  private handleError(error: any): Observable<any> {
    console.error('API Error:', error); // Log the error for debugging
    return throwError(() => new Error(error.message || 'Server error'));
  }
}
    console.error('API Error:', error); // Log the error for debugging
    return throwError(() => new Error(error.message || 'Server error'));
  }
}
