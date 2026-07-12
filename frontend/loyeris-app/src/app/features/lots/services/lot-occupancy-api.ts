import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface AvailableTenant {
  email: string | null;
  firstName: string;
  id: string;
  isCurrent: boolean;
  lastName: string;
}

export interface LotOccupancy {
  chargesCents: number;
  depositCents: number;
  endsOn: string | null;
  leaseId: string;
  lotId: string;
  notes: string | null;
  paymentTerms: string | null;
  rentDueDay: number;
  rentExcludingChargesCents: number;
  startsOn: string;
  tenantFirstName: string;
  tenantId: string;
  tenantLastName: string;
}

export interface SaveLotOccupancyRequest {
  chargesCents: number;
  depositCents: number;
  endsOn: string | null;
  notes: string | null;
  paymentTerms: string | null;
  rentDueDay: number;
  rentExcludingChargesCents: number;
  startsOn: string | null;
  tenantId: string | null;
}

export type UpdateLotLeaseRequest = Omit<SaveLotOccupancyRequest, 'tenantId'> & {
  startsOn: string;
};

@Injectable({ providedIn: 'root' })
export class LotOccupancyApi {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api/leasing';

  listAvailableTenants(lotId?: string): Observable<readonly AvailableTenant[]> {
    const params = lotId ? new HttpParams().set('lotId', lotId) : undefined;
    return this.http.get<readonly AvailableTenant[]>(`${this.baseUrl}/available-tenants`, { params });
  }

  list(): Observable<readonly LotOccupancy[]> {
    return this.http.get<readonly LotOccupancy[]>(`${this.baseUrl}/lot-occupancies`);
  }

  get(lotId: string): Observable<LotOccupancy | null> {
    return this.http.get<LotOccupancy | null>(`${this.baseUrl}/lots/${lotId}/occupancy`);
  }

  listLeases(lotId: string): Observable<readonly LotOccupancy[]> {
    return this.http.get<readonly LotOccupancy[]>(`${this.baseUrl}/lots/${lotId}/leases`);
  }

  save(lotId: string, request: SaveLotOccupancyRequest): Observable<LotOccupancy | null> {
    return this.http.put<LotOccupancy | null>(`${this.baseUrl}/lots/${lotId}/occupancy`, request);
  }

  updateLease(
    lotId: string,
    leaseId: string,
    request: UpdateLotLeaseRequest,
  ): Observable<LotOccupancy> {
    return this.http.put<LotOccupancy>(
      `${this.baseUrl}/lots/${lotId}/leases/${leaseId}`,
      request,
    );
  }

  deleteLease(lotId: string, leaseId: string): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/lots/${lotId}/leases/${leaseId}`);
  }
}
