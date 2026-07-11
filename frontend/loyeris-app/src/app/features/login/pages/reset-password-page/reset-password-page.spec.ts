import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { AuthSession } from '../../../../core/auth/auth-session';
import { ResetPasswordPage } from './reset-password-page';

describe('ResetPasswordPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ResetPasswordPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'reset-password', component: ResetPasswordPage }]),
      ],
    }).compileComponents();
  });

  it('should validate the token before displaying the password form', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/reset-password?token=raw-token', ResetPasswordPage);
    const http = TestBed.inject(HttpTestingController);

    expect(harness.routeNativeElement?.textContent).toContain('Vérification du lien');
    const request = http.expectOne('/api/identity-access/password-reset-validations');
    expect(request.request.body).toEqual({ token: 'raw-token' });
    request.flush(null);
    harness.detectChanges();

    expect(harness.routeNativeElement?.querySelector('#new-password')).toBeTruthy();
  });

  it('should render an unusable-link state for a rejected token', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/reset-password?token=expired-token', ResetPasswordPage);

    TestBed.inject(HttpTestingController)
      .expectOne('/api/identity-access/password-reset-validations')
      .flush({}, { status: 422, statusText: 'Unprocessable Entity' });
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain('Lien inutilisable');
    expect(harness.routeNativeElement?.querySelector('a[href="/forgot-password"]')).toBeTruthy();
  });

  it('should submit the new password without its confirmation', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/reset-password?token=raw-token', ResetPasswordPage);
    const http = TestBed.inject(HttpTestingController);
    const clearSession = vi.spyOn(TestBed.inject(AuthSession), 'clear');
    http.expectOne('/api/identity-access/password-reset-validations').flush(null);
    harness.detectChanges();

    setInputValue(harness.routeNativeElement!, '#new-password', 'NewValid1!password');
    setInputValue(
      harness.routeNativeElement!,
      '#new-password-confirmation',
      'NewValid1!password',
    );
    harness.routeNativeElement?.querySelector('form')?.dispatchEvent(new Event('submit'));

    const request = http.expectOne('/api/identity-access/password-resets');
    expect(request.request.body).toEqual({
      token: 'raw-token',
      newPassword: 'NewValid1!password',
    });
    request.flush(null);
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain('Mot de passe réinitialisé');
    expect(clearSession).toHaveBeenCalledOnce();
  });

  it('should reject mismatched passwords without calling the API', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/reset-password?token=raw-token', ResetPasswordPage);
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/identity-access/password-reset-validations').flush(null);
    harness.detectChanges();

    setInputValue(harness.routeNativeElement!, '#new-password', 'NewValid1!password');
    setInputValue(harness.routeNativeElement!, '#new-password-confirmation', 'Different1!password');
    harness.routeNativeElement?.querySelector('form')?.dispatchEvent(new Event('submit'));
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain(
      'Les deux mots de passe doivent être identiques',
    );
    http.expectNone('/api/identity-access/password-resets');
  });
});

function setInputValue(element: HTMLElement, selector: string, value: string): void {
  const input = element.querySelector(selector) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
}
