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

export interface LoginRequest {
  email: string;
  password: string;
  rememberMe: boolean;
}

export interface AuthenticatedUser {
  email: string;
  firstName: string;
  lastName: string;
  userId: string;
}

export interface AuthenticationResponse extends AuthenticatedUser {
  accessToken: string;
  accessTokenExpiresAt: string;
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

  login(request: LoginRequest): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${this.baseUrl}/auth/login`, request, {
      withCredentials: true,
    });
  }

  refresh(): Observable<AuthenticationResponse> {
    return this.http.post<AuthenticationResponse>(`${this.baseUrl}/auth/refresh`, null, {
      withCredentials: true,
    });
  }

  logout(): Observable<void> {
    return this.http.post<void>(`${this.baseUrl}/auth/logout`, null, { withCredentials: true });
  }
}
