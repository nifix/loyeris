import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export type SciStatus = 'Active' | 'Archived';
export type TaxRegime = 'IR';

export interface Sci {
  archivedAt: string | null;
  city: string | null;
  country: string;
  createdAt: string;
  id: string;
  incorporatedOn: string | null;
  name: string;
  postalCode: string | null;
  siren: string | null;
  status: SciStatus;
  street: string | null;
  taxRegime: TaxRegime;
  updatedAt: string;
  workspaceId: string;
}

export interface SaveSciRequest {
  city: string | null;
  country: string;
  incorporatedOn: string | null;
  name: string;
  postalCode: string | null;
  siren: string | null;
  status: SciStatus;
  street: string | null;
  taxRegime: TaxRegime;
}

export type CreateSciRequest = SaveSciRequest;
export type UpdateSciRequest = SaveSciRequest;

@Injectable({ providedIn: 'root' })
export class SciApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/portfolio/scis';

  list(): Observable<readonly Sci[]> {
    return this.http.get<readonly Sci[]>(this.baseUrl);
  }

  get(sciId: string): Observable<Sci> {
    return this.http.get<Sci>(`${this.baseUrl}/${sciId}`);
  }

  create(request: CreateSciRequest): Observable<Sci> {
    return this.http.post<Sci>(this.baseUrl, request);
  }

  update(sciId: string, request: UpdateSciRequest): Observable<Sci> {
    return this.http.put<Sci>(`${this.baseUrl}/${sciId}`, request);
  }
}
