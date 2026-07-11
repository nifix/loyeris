import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import {
  catchError,
  finalize,
  map,
  Observable,
  shareReplay,
  switchMap,
  throwError,
} from 'rxjs';

import { AuthSession } from './auth-session';

let refreshInFlight: Observable<string> | null = null;

export const authInterceptor: HttpInterceptorFn = (request, next) => {
  const authSession = inject(AuthSession);
  const router = inject(Router);
  const isApiRequest = request.url.startsWith('/api/');
  const isAuthenticationRequest = request.url.includes('/auth/');
  const accessToken = authSession.accessToken();
  const authenticatedRequest =
    isApiRequest && accessToken
      ? request.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
      : request;

  return next(authenticatedRequest).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || !isApiRequest || isAuthenticationRequest) {
        return throwError(() => error);
      }

      if (!refreshInFlight) {
        refreshInFlight = authSession.refresh().pipe(
          map((response) => response.accessToken),
          finalize(() => (refreshInFlight = null)),
          shareReplay({ bufferSize: 1, refCount: false }),
        );
      }

      return refreshInFlight.pipe(
        catchError((refreshError) => {
          authSession.clear();
          void router.navigate(['/login']);
          return throwError(() => refreshError);
        }),
        switchMap((newAccessToken) =>
          next(request.clone({ setHeaders: { Authorization: `Bearer ${newAccessToken}` } })),
        ),
      );
    }),
  );
};
