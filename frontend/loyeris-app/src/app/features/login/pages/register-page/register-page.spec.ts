import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { RegisterPage } from './register-page';

describe('RegisterPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterPage],
      providers: [
        provideHttpClient(),
        provideHttpClientTesting(),
        provideRouter([{ path: 'register', component: RegisterPage }]),
      ],
    }).compileComponents();
  });

  it('should create the register page', () => {
    const fixture = TestBed.createComponent(RegisterPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the account creation form and its reassurance content', () => {
    const fixture = TestBed.createComponent(RegisterPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Créer votre compte');
    expect(compiled.textContent).toContain('Votre espace Loyeris commence ici');
    expect(compiled.querySelector('ui-feature-card')).toBeNull();
    expect(compiled.querySelectorAll('input[type="text"]')).toHaveLength(2);
    expect(compiled.querySelector('input[type="email"]')).toBeTruthy();
    expect(compiled.querySelectorAll('input[type="password"]')).toHaveLength(2);
    expect(compiled.querySelector('a[href="/login"]')).toBeTruthy();
    expect(compiled.querySelector('[role="alert"]')).toBeNull();
  });

  it('should render a submission error next to the primary action', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/register?error=email-exists', RegisterPage);

    const alert = harness.routeNativeElement?.querySelector('[role="alert"]');
    expect(alert?.textContent).toContain('Cette adresse email est déjà utilisée');
    expect(alert?.querySelector('a[href="/login"]')).toBeTruthy();
  });

  it('should render the generic submission error without a contextual action', async () => {
    const harness = await RouterTestingHarness.create();
    await harness.navigateByUrl('/register?error=generic', RegisterPage);

    const alert = harness.routeNativeElement?.querySelector('[role="alert"]');
    expect(alert?.textContent).toContain('Création du compte impossible');
    expect(alert?.querySelector('a')).toBeNull();
  });

  it('should not submit an invalid form', () => {
    const fixture = TestBed.createComponent(RegisterPage);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    TestBed.inject(HttpTestingController).expectNone('/api/identity-access/accounts');
    expect(fixture.nativeElement.textContent).toContain('Ce champ est obligatoire.');
  });

  it('should update password requirements while the user types', () => {
    const fixture = TestBed.createComponent(RegisterPage);
    fixture.detectChanges();

    const requirements = () =>
      Array.from(
        fixture.nativeElement.querySelectorAll('ui-password-checklist [data-met]'),
      ) as HTMLElement[];
    expect(requirements().map((requirement) => requirement.dataset['met'])).toEqual([
      'false',
      'false',
      'false',
    ]);

    setInputValue(fixture.nativeElement, '#register-password', 'motdepasse');
    fixture.detectChanges();
    expect(requirements().map((requirement) => requirement.dataset['met'])).toEqual([
      'true',
      'false',
      'false',
    ]);

    setInputValue(fixture.nativeElement, '#register-password', 'Loyeris1!');
    fixture.detectChanges();
    expect(requirements().map((requirement) => requirement.dataset['met'])).toEqual([
      'true',
      'true',
      'true',
    ]);
  });

  it('should submit the backend payload once and redirect to the pending state', async () => {
    const fixture = TestBed.createComponent(RegisterPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.detectChanges();
    fillRegistrationForm(fixture.nativeElement);

    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;
    form.dispatchEvent(new Event('submit'));
    form.dispatchEvent(new Event('submit'));
    const http = TestBed.inject(HttpTestingController);
    const request = http.expectOne('/api/identity-access/accounts');

    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      firstName: 'Camille',
      lastName: 'Robert',
      email: 'camille@example.fr',
      password: 'Valid1!password',
      termsAccepted: true,
    });
    expect(request.request.body.passwordConfirmation).toBeUndefined();
    http.expectNone('/api/identity-access/accounts');
    request.flush({
      userId: 'f4910062-7e20-4e08-ad07-f09ad03948e6',
      email: 'camille@example.fr',
      verificationRequired: true,
    });
    await fixture.whenStable();

    expect(navigate).toHaveBeenCalledWith(['/verify-email'], {
      queryParams: { status: 'pending' },
    });
  });

  it('should map an email conflict to the contextual error', () => {
    const fixture = TestBed.createComponent(RegisterPage);
    fixture.detectChanges();
    fillRegistrationForm(fixture.nativeElement);
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    TestBed.inject(HttpTestingController)
      .expectOne('/api/identity-access/accounts')
      .flush({}, { status: 409, statusText: 'Conflict' });
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Cette adresse email est déjà utilisée');
  });
});

function fillRegistrationForm(element: HTMLElement): void {
  setInputValue(element, '#first-name', 'Camille');
  setInputValue(element, '#last-name', 'Robert');
  setInputValue(element, '#register-email', 'camille@example.fr');
  setInputValue(element, '#register-password', 'Valid1!password');
  setInputValue(element, '#password-confirmation', 'Valid1!password');
  const checkbox = element.querySelector('input[type="checkbox"]') as HTMLInputElement;
  checkbox.click();
}

function setInputValue(element: HTMLElement, selector: string, value: string): void {
  const input = element.querySelector(selector) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
}
