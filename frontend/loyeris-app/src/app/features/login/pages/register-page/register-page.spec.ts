import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { RouterTestingHarness } from '@angular/router/testing';

import { RegisterPage } from './register-page';

describe('RegisterPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [RegisterPage],
      providers: [provideRouter([{ path: 'register', component: RegisterPage }])],
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
});
