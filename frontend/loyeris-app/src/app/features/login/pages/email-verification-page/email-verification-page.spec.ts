import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { EmailVerificationPage } from './email-verification-page';

describe('EmailVerificationPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [EmailVerificationPage],
      providers: [
        provideRouter([
          {
            path: 'verify-email',
            component: EmailVerificationPage,
          },
        ]),
      ],
    }).compileComponents();
  });

  it('should render the success state for a verified token', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/verify-email?token=verified-token', EmailVerificationPage);

    expect(harness.routeNativeElement?.textContent).toContain('Votre adresse email est vérifiée');
    expect(harness.routeNativeElement?.querySelector('a[href="/login"]')).toBeTruthy();
  });

  it('should render the refusal state when validation fails', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl(
      '/verify-email?token=expired-token&status=error',
      EmailVerificationPage,
    );

    expect(harness.routeNativeElement?.textContent).toContain(
      'Ce lien de vérification n’est plus valide',
    );
  });
});
