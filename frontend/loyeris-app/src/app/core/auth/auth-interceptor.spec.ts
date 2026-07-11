import { HttpClient, provideHttpClient, withInterceptors } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { authInterceptor } from './auth-interceptor';
import { AuthSession } from './auth-session';

describe('authInterceptor', () => {
  beforeEach(() => {
    TestBed.configureTestingModule({
      providers: [
        provideHttpClient(withInterceptors([authInterceptor])),
        provideHttpClientTesting(),
        provideRouter([]),
      ],
    });
  });

  it('should attach the JWT and rotate it after an unauthorized API response', () => {
    const session = TestBed.inject(AuthSession);
    const http = TestBed.inject(HttpTestingController);

    session
      .login({ email: 'camille@example.fr', password: 'Valid1!password', rememberMe: true })
      .subscribe();
    http.expectOne('/api/identity-access/auth/login').flush(authenticationResponse('old-token'));

    let response: unknown;
    TestBed.inject(HttpClient)
      .get('/api/portfolio/scis')
      .subscribe((value) => (response = value));
    const firstRequest = http.expectOne('/api/portfolio/scis');
    expect(firstRequest.request.headers.get('Authorization')).toBe('Bearer old-token');
    firstRequest.flush({}, { status: 401, statusText: 'Unauthorized' });

    http.expectOne('/api/identity-access/auth/refresh').flush(authenticationResponse('new-token'));
    const retriedRequest = http.expectOne('/api/portfolio/scis');
    expect(retriedRequest.request.headers.get('Authorization')).toBe('Bearer new-token');
    retriedRequest.flush([]);

    expect(response).toEqual([]);
  });
});

function authenticationResponse(accessToken: string) {
  return {
    accessToken,
    accessTokenExpiresAt: '2026-07-11T22:00:00Z',
    userId: 'user-id',
    email: 'camille@example.fr',
    firstName: 'Camille',
    lastName: 'Robert',
  };
}
