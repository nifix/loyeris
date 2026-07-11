import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';

import { ForgotPasswordPage } from './forgot-password-page';

describe('ForgotPasswordPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ForgotPasswordPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'forgot-password', component: ForgotPasswordPage }]),
      ],
    }).compileComponents();
  });

  it('should not submit an invalid email', () => {
    const fixture = TestBed.createComponent(ForgotPasswordPage);
    fixture.detectChanges();

    setInputValue(fixture.nativeElement, '#forgot-password-email', 'not-an-email');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Saisissez une adresse email valide');
    TestBed.inject(HttpTestingController).expectNone(
      '/api/identity-access/password-reset-requests',
    );
  });

  it('should submit the normalized email and render the neutral confirmation', () => {
    const fixture = TestBed.createComponent(ForgotPasswordPage);
    fixture.detectChanges();

    setInputValue(fixture.nativeElement, '#forgot-password-email', ' camille@example.fr ');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    const request = TestBed.inject(HttpTestingController).expectOne(
      '/api/identity-access/password-reset-requests',
    );
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({ email: 'camille@example.fr' });
    request.flush(null);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Consultez votre boîte mail');
    expect(fixture.nativeElement.textContent).toContain('Si un compte Loyeris correspond');
  });

  it('should prevent duplicate reset requests while submitting', () => {
    const fixture = TestBed.createComponent(ForgotPasswordPage);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#forgot-password-email', 'camille@example.fr');

    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));
    form.dispatchEvent(new Event('submit'));

    TestBed.inject(HttpTestingController).expectOne(
      '/api/identity-access/password-reset-requests',
    );
  });
});

function setInputValue(element: HTMLElement, selector: string, value: string): void {
  const input = element.querySelector(selector) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
}
