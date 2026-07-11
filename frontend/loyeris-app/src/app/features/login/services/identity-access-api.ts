import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';

export interface RegisterAccountRequest {
  email: string;
  firstName: string;
  lastName: string;
  password: string;
  termsAccepted: boolean;
}

export interface RegisteredAccount {
  email: string;
  userId: string;
  verificationRequired: boolean;
}

@Injectable({ providedIn: 'root' })
export class IdentityAccessApi {
  private readonly http = inject(HttpClient);

  // Relative URLs use the Angular proxy locally and remain same-origin in production.
  private readonly baseUrl = '/api/identity-access';

  registerAccount(request: RegisterAccountRequest): Observable<RegisteredAccount> {
    return this.http.post<RegisteredAccount>(`${this.baseUrl}/accounts`, request);
  }

  verifyEmail(token: string): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/email-verifications`, { token });
  }
}
