import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ScisPage } from './scis-page';

describe('ScisPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScisPage],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();
  });

  it('should create the SCI portfolio page', () => {
    const fixture = TestBed.createComponent(ScisPage);

    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should load and display only the SCI data returned by the API', () => {
    const fixture = TestBed.createComponent(ScisPage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    const request = http.expectOne('/api/portfolio/scis');
    expect(request.request.method).toBe('GET');
    request.flush([
      createSci({
        id: 'sci-2',
        name: 'SCI Carnot',
        status: 'Archived',
        siren: null,
        archivedAt: '2026-06-30T08:00:00Z',
      }),
      createSci({ id: 'sci-1', name: 'SCI Les Tilleuls', status: 'Active' }),
    ]);
    http.expectOne('/api/portfolio/lots').flush([
      createLot({ id: 'lot-1', sciId: 'sci-1', reference: 'A01' }),
      createLot({ id: 'lot-2', sciId: 'sci-1', reference: 'A02' }),
    ]);
    http.expectOne('/api/leasing/lot-occupancies').flush([
      {
        lotId: 'lot-1',
        leaseId: 'lease-1',
        tenantId: 'tenant-1',
        tenantFirstName: 'Camille',
        tenantLastName: 'Robert',
        startsOn: currentMonthDate(1),
        endsOn: null,
        rentDueDay: 5,
        rentExcludingChargesCents: 65000,
        chargesCents: 5000,
        depositCents: 65000,
        paymentTerms: null,
        notes: null,
      },
    ]);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const cards = compiled.querySelectorAll('ui-sci-card');

    expect(compiled.textContent).toContain('2 structures suivies');
    expect(compiled.textContent).not.toContain('Workspace courant');
    expect(compiled.textContent).toContain('SCI Les Tilleuls');
    expect(compiled.textContent).not.toContain('SCI Carnot');
    expect(compiled.textContent).toContain('Afficher les SCI archivées');
    expect(compiled.textContent).toContain('10/01/2024');
    expect(cards).toHaveLength(1);
    expect(cards[0].textContent).toContain('SCI Les Tilleuls');
    expect(cards[0].textContent).toContain('1 400,00');
    expect(cards[0].textContent).toContain('Potentiel mensuel');
    expect(cards[0].textContent).toContain('sans prorata');
    expect(cards[0].textContent).toContain('2');
    expect(cards[0].textContent).toContain('1');
    expect(cards[0].textContent).toContain('50 %');
    expect(compiled.querySelector('a[href="/scis/sci-1/edit"]')).toBeTruthy();
    expect(compiled.textContent).not.toContain('4 320 €');

    const archivedToggle = [...compiled.querySelectorAll('button')]
      .find((button) => button.textContent?.includes('Afficher les SCI archivées'))!;
    archivedToggle.click();
    fixture.detectChanges();

    const expandedCards = compiled.querySelectorAll('ui-sci-card');
    expect(expandedCards).toHaveLength(2);
    expect(expandedCards[1].textContent).toContain('SCI Carnot');
    expect(expandedCards[1].textContent).toContain('Archivée le');
    expect(expandedCards[1].textContent).toContain('30/06/2026');
    expect(expandedCards[1].textContent).toContain('indicateurs opérationnels sont masqués');
    expect(compiled.querySelector('[aria-label="SCI archivées"]')).toBeTruthy();
  });

  it('should render an empty state when the workspace has no SCI', () => {
    const fixture = TestBed.createComponent(ScisPage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/scis').flush([]);
    http.expectOne('/api/portfolio/lots').flush([]);
    http.expectOne('/api/leasing/lot-occupancies').flush([]);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Ajoutez votre première SCI');
    expect(fixture.nativeElement.querySelector('a[href="/scis/new"]')).toBeTruthy();
  });

  it('should dismiss the creation confirmation after five seconds', () => {
    vi.useFakeTimers();
    const fixture = TestBed.createComponent(ScisPage);
    fixture.componentRef.setInput('presentationScis', []);
    fixture.componentRef.setInput('presentationState', 'loaded');
    fixture.componentRef.setInput('creationSucceeded', true);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('La SCI a bien été ajoutée');

    vi.advanceTimersByTime(5_000);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).not.toContain('La SCI a bien été ajoutée');
    fixture.destroy();
    vi.useRealTimers();
  });
});

function createSci(overrides: Record<string, unknown> = {}) {
  return {
    id: 'sci-id',
    workspaceId: 'workspace-id',
    name: 'SCI Test',
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
    ...overrides,
  };
}

function createLot(overrides: Record<string, unknown> = {}) {
  return {
    id: 'lot-id',
    sciId: 'sci-id',
    sciName: 'SCI Test',
    reference: 'Lot A01',
    type: 'T2',
    status: 'Active',
    street: '12 rue des Tilleuls',
    postalCode: '69000',
    city: 'Lyon',
    country: 'FR',
    surfaceSqm: 42,
    potentialRentExcludingChargesCents: 65000,
    potentialChargesCents: 5000,
    suggestedDepositCents: 65000,
    notes: null,
    createdAt: '2026-07-12T08:00:00Z',
    updatedAt: '2026-07-12T08:00:00Z',
    archivedAt: null,
    ...overrides,
  };
}

function currentMonthDate(day: number): string {
  const currentDate = new Date();
  const year = currentDate.getFullYear();
  const month = String(currentDate.getMonth() + 1).padStart(2, '0');
  return `${year}-${month}-${String(day).padStart(2, '0')}`;
}
