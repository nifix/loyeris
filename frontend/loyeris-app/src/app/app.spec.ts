import { provideHttpClient } from '@angular/common/http';
import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';
import { of } from 'rxjs';
import { App } from './app';
import { routes } from './app.routes';
import { AuthSession } from './core/auth/auth-session';
import { LotApi } from './features/lots/services/lot-api';
import { LotOccupancyApi } from './features/lots/services/lot-occupancy-api';
import { SciApi } from './features/scis/services/sci-api';

describe('App', () => {
  const authenticated = signal(false);
  const sci = {
    id: 'sci-id',
    workspaceId: 'workspace-id',
    name: 'SCI Les Tilleuls',
    siren: '123456789',
    taxRegime: 'IR',
    status: 'Active',
    street: '12 rue des Tilleuls',
    postalCode: '69000',
    city: 'Lyon',
    country: 'FR',
    incorporatedOn: '2024-01-10',
    createdAt: '2026-07-12T08:00:00Z',
    updatedAt: '2026-07-12T08:00:00Z',
    archivedAt: null,
  };
  const lot = {
    id: 'lot-a01',
    sciId: 'sci-id',
    sciName: 'SCI Les Tilleuls',
    reference: 'Lot A01',
    type: 'T2',
    status: 'Active',
    street: '12 rue des Tilleuls',
    postalCode: '69000',
    city: 'Lyon',
    country: 'FR',
    surfaceSqm: 42.5,
    potentialRentExcludingChargesCents: 65000,
    potentialChargesCents: 5000,
    suggestedDepositCents: 65000,
    notes: null,
    createdAt: '2026-07-12T08:00:00Z',
    updatedAt: '2026-07-12T08:00:00Z',
    archivedAt: null,
  };

  beforeEach(async () => {
    authenticated.set(false);
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        provideHttpClient(),
        provideRouter(routes),
        {
          provide: AuthSession,
          useValue: {
            accessToken: () => (authenticated() ? 'access-token' : null),
            authenticated: authenticated.asReadonly(),
            user: () =>
              authenticated()
                ? {
                    userId: 'user-id',
                    email: 'camille@example.fr',
                    firstName: 'Camille',
                    lastName: 'Robert',
                  }
                : null,
            logout: () => of(undefined),
          },
        },
        {
          provide: SciApi,
          useValue: {
            list: () => of([sci]),
            get: () => of(sci),
          },
        },
        {
          provide: LotApi,
          useValue: { list: () => of([lot]), get: () => of(lot) },
        },
        {
          provide: LotOccupancyApi,
          useValue: {
            list: () => of([]),
            get: () => of(null),
            listLeases: () => of([]),
            listAvailableTenants: () => of([]),
          },
        },
      ],
    }).compileComponents();
  });

  it('should create the app', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('should render the login page', async () => {
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/login');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-login-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Connexion');
  });

  it('should render the register page', async () => {
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/register');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-register-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Créer votre compte');
  });

  it('should render the email verification page', async () => {
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/verify-email?status=pending');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-email-verification-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Consultez votre boîte mail');
  });

  it('should render the dashboard page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/dashboard');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-dashboard-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Loyers attendus');
  });

  it('should render the SCI page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/scis');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-scis-page')).toBeTruthy();
    expect(compiled.textContent).toContain('SCI Les Tilleuls');
  });

  it('should render the SCI edit page with the existing values', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/scis/sci-id/edit');
    fixture.detectChanges();
    await fixture.whenStable();
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-create-sci-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Modifier la SCI');
    expect((compiled.querySelector('#sci-name') as HTMLInputElement).value).toBe('SCI Les Tilleuls');
  });

  it('should navigate between authenticated pages without reloading the application', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/dashboard');
    fixture.detectChanges();
    await fixture.whenStable();

    const sciLink = fixture.nativeElement.querySelector(
      'ui-app-shell a[href="/scis"]',
    ) as HTMLAnchorElement;
    sciLink.click();
    fixture.detectChanges();
    await fixture.whenStable();

    expect(router.url).toBe('/scis');
    expect(authenticated()).toBe(true);
    expect((fixture.nativeElement as HTMLElement).querySelector('app-scis-page')).toBeTruthy();
  });

  it('should render the lots page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/lots');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-lots-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Lot A01');
  });

  it('should render the lot creation page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/lots/new');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-lot-form-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Ajouter un lot');
    expect(compiled.textContent).toContain('Assigner un locataire');
  });

  it('should render the lot details in read-only mode', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/lots/a02');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-lot-form-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Détails du lot');
    expect(compiled.textContent).toContain('Modifier le lot');
  });

  it('should render the tenants page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/tenants');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-tenants-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Liste des locataires');
  });

  it('should render the tenant detail page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/tenants/camille-robert');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-tenant-detail-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Coordonnées');
  });

  it('should render the rents page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/rents');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-rents-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Échéances du mois');
  });

  it('should render the settings page', async () => {
    authenticated.set(true);
    const fixture = TestBed.createComponent(App);
    const router = TestBed.inject(Router);

    await router.navigateByUrl('/settings');
    fixture.detectChanges();
    await fixture.whenStable();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('app-settings-page')).toBeTruthy();
    expect(compiled.textContent).toContain('Enregistrer le profil');
  });
});
