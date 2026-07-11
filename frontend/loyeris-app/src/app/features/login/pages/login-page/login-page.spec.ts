import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { LoginPage } from './login-page';

describe('LoginPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'login', component: LoginPage }]),
      ],
    }).compileComponents();
  });

  it('should create the login page', () => {
    const fixture = TestBed.createComponent(LoginPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the complete login value proposition and form', () => {
    const fixture = TestBed.createComponent(LoginPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Le cockpit locatif pensé pour remplacer Excel');
    expect(compiled.textContent).toContain('Pages déjà maquettées');
    expect(compiled.querySelector('input[type="email"]')).toBeTruthy();
    expect(compiled.querySelector('input[type="password"]')).toBeTruthy();
    expect(compiled.querySelector('a[href="/register"]')).toBeTruthy();
    expect(compiled.querySelector('[role="alert"]')).toBeNull();
  });

  it('should render a neutral error for an unknown account', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/login?error=account-not-found', LoginPage);

    const alert = harness.routeNativeElement?.querySelector('[role="alert"]');
    expect(alert?.textContent).toContain('Identifiants incorrects');
    expect(alert?.textContent).not.toContain('Aucun compte');
  });

  it('should render the unverified email error', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/login?error=email-not-verified', LoginPage);

    expect(harness.routeNativeElement?.querySelector('[role="alert"]')?.textContent).toContain(
      'Adresse email non vérifiée',
    );
  });

  it('should submit credentials once and navigate after authentication', async () => {
    const fixture = TestBed.createComponent(LoginPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#email', 'camille@example.fr');
    setInputValue(fixture.nativeElement, '#password', 'Valid1!password');

    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));
    form.dispatchEvent(new Event('submit'));
    const request = TestBed.inject(HttpTestingController).expectOne(
      '/api/identity-access/auth/login',
    );
    expect(request.request.body).toEqual({
      email: 'camille@example.fr',
      password: 'Valid1!password',
      rememberMe: true,
    });
    expect(request.request.withCredentials).toBe(true);
    request.flush({
      accessToken: 'access-token',
      accessTokenExpiresAt: '2026-07-11T22:00:00Z',
      userId: 'user-id',
      email: 'camille@example.fr',
      firstName: 'Camille',
      lastName: 'Robert',
    });
    await fixture.whenStable();

    expect(navigate).toHaveBeenCalledWith(['/dashboard']);
  });

  it('should explain that a fresh verification email was sent', () => {
    const fixture = TestBed.createComponent(LoginPage);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#email', 'camille@example.fr');
    setInputValue(fixture.nativeElement, '#password', 'Valid1!password');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    TestBed.inject(HttpTestingController)
      .expectOne('/api/identity-access/auth/login')
      .flush(
        { errorCode: 'identity.email_not_verified' },
        { status: 403, statusText: 'Forbidden' },
      );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Un nouveau lien de vérification');
  });
});

function setInputValue(element: HTMLElement, selector: string, value: string): void {
  const input = element.querySelector(selector) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
}
