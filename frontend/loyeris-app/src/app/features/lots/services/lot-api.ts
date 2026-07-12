import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export type LotStatus = 'Active' | 'Archived';
export type LotType = 'Studio' | 'T1' | 'T2' | 'T3' | 'T4' | 'T5' | 'Garage' | 'Local' | 'Other';

export interface Lot {
  archivedAt: string | null;
  city: string;
  country: string;
  createdAt: string;
  id: string;
  notes: string | null;
  postalCode: string;
  potentialChargesCents: number;
  potentialRentExcludingChargesCents: number;
  reference: string;
  sciId: string;
  sciName: string;
  status: LotStatus;
  street: string;
  suggestedDepositCents: number;
  surfaceSqm: number | null;
  type: LotType;
  updatedAt: string;
}

export interface SaveLotRequest {
  city: string;
  country: string;
  notes: string | null;
  postalCode: string;
  potentialChargesCents: number;
  potentialRentExcludingChargesCents: number;
  reference: string;
  sciId: string;
  status: LotStatus;
  street: string;
  suggestedDepositCents: number;
  surfaceSqm: number | null;
  type: LotType;
}

@Injectable({ providedIn: 'root' })
export class LotApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/portfolio/lots';

  list(): Observable<readonly Lot[]> {
    return this.http.get<readonly Lot[]>(this.baseUrl);
  }

  get(lotId: string): Observable<Lot> {
    return this.http.get<Lot>(`${this.baseUrl}/${lotId}`);
  }

  create(request: SaveLotRequest): Observable<Lot> {
    return this.http.post<Lot>(this.baseUrl, request);
  }

  update(lotId: string, request: SaveLotRequest): Observable<Lot> {
    return this.http.put<Lot>(`${this.baseUrl}/${lotId}`, request);
  }
}
