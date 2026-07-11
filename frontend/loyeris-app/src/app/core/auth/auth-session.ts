import { computed, inject, Injectable, signal } from '@angular/core';
import { catchError, finalize, map, Observable, of, tap } from 'rxjs';

import {
  type AuthenticatedUser,
  type AuthenticationResponse,
  IdentityAccessApi,
  type LoginRequest,
} from './identity-access-api';

@Injectable({ providedIn: 'root' })
export class AuthSession {
  private readonly api = inject(IdentityAccessApi);
  private readonly accessTokenState = signal<string | null>(null);
  private readonly userState = signal<AuthenticatedUser | null>(null);

  readonly accessToken = this.accessTokenState.asReadonly();
  readonly user = this.userState.asReadonly();
  readonly authenticated = computed(() => this.accessTokenState() !== null);

  login(request: LoginRequest): Observable<AuthenticationResponse> {
    return this.api.login(request).pipe(tap((response) => this.applyAuthentication(response)));
  }

  refresh(): Observable<AuthenticationResponse> {
    return this.api.refresh().pipe(tap((response) => this.applyAuthentication(response)));
  }

  restore(): Observable<boolean> {
    return this.refresh().pipe(
      map(() => true),
      catchError(() => {
        this.clear();
        return of(false);
      }),
    );
  }

  logout(): Observable<void> {
    return this.api.logout().pipe(finalize(() => this.clear()));
  }

  clear(): void {
    this.accessTokenState.set(null);
    this.userState.set(null);
  }

  private applyAuthentication(response: AuthenticationResponse): void {
    this.accessTokenState.set(response.accessToken);
    this.userState.set({
      userId: response.userId,
      email: response.email,
      firstName: response.firstName,
      lastName: response.lastName,
    });
  }
}
