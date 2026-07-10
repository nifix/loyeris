import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { LoginPage } from './login-page';

describe('LoginPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LoginPage],
      providers: [provideRouter([{ path: 'login', component: LoginPage }])],
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
});
