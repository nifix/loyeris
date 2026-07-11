import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { EmailVerificationPage } from './email-verification-page';

describe('EmailVerificationPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmailVerificationPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'verify-email', component: EmailVerificationPage }]),
      ],
    }).compileComponents();
  });

  it('should verify the token before rendering the success state', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/verify-email?token=verified-token', EmailVerificationPage);
    const http = TestBed.inject(HttpTestingController);

    expect(harness.routeNativeElement?.textContent).toContain('Vérification en cours');
    const request = http.expectOne('/api/identity-access/email-verifications');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ token: 'verified-token' });
    request.flush(null);
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain('Votre adresse email est vérifiée');
    expect(harness.routeNativeElement?.querySelector('a[href="/login"]')).toBeTruthy();
  });

  it('should render the refusal state when the API rejects the token', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/verify-email?token=expired-token', EmailVerificationPage);
    const request = TestBed.inject(HttpTestingController).expectOne(
      '/api/identity-access/email-verifications',
    );

    request.flush({}, { status: 422, statusText: 'Unprocessable Entity' });
    harness.detectChanges();

    expect(harness.routeNativeElement?.textContent).toContain(
      'Ce lien de vérification n’est plus valide',
    );
  });

  it('should render the pending state without calling the API', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/verify-email?status=pending', EmailVerificationPage);

    expect(harness.routeNativeElement?.textContent).toContain('Consultez votre boîte mail');
    TestBed.inject(HttpTestingController).expectNone('/api/identity-access/email-verifications');
  });
});
