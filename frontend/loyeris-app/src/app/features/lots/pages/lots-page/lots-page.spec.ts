import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { type Lot } from '../../services/lot-api';
import { LotsPage } from './lots-page';

describe('LotsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LotsPage],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();
  });

  it('should load real lots and their current occupancies', () => {
    const fixture = TestBed.createComponent(LotsPage);
    const router = TestBed.inject(Router);
    const navigateByUrl = vi.spyOn(router, 'navigateByUrl').mockResolvedValue(true);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/lots').flush([
      createLot({ id: 'archived', reference: 'Lot Z01', status: 'Archived' }),
      createLot({ id: 'archived-empty', reference: 'Lot Z02', status: 'Archived' }),
      createLot({
        id: 'active',
        reference: 'Lot A01',
        potentialRentExcludingChargesCents: 72000,
      }),
    ]);
    http.expectOne('/api/leasing/lot-occupancies').flush([
      {
        lotId: 'active',
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
      {
        lotId: 'archived',
        leaseId: 'archived-lease',
        tenantId: 'archived-tenant',
        tenantFirstName: 'Ancien',
        tenantLastName: 'Locataire',
        startsOn: currentMonthDate(1),
        endsOn: currentMonthDate(1),
        rentDueDay: 5,
        rentExcludingChargesCents: 50000,
        chargesCents: 5000,
        depositCents: 50000,
        paymentTerms: null,
        notes: null,
      },
    ]);
    fixture.detectChanges();

    const rows = fixture.nativeElement.querySelectorAll('ui-lots-table tbody tr');
    expect(rows).toHaveLength(2);
    expect(rows[0].textContent).toContain('Lot A01');
    expect(rows[0].textContent).toContain('Camille Robert');
    expect(rows[0].textContent).toContain('650');
    expect(rows[0].textContent).toContain('50');
    expect(rows[0].textContent).toContain('700');
    expect(rows[0].textContent).not.toContain('720');
    expect(fixture.nativeElement.textContent).toContain('Potentiel mensuel du parc');
    expect(rows[1].textContent).toContain('Lot Z01');
    expect(rows[1].textContent).toContain('Archivé');
    expect(rows[1].textContent).not.toContain('Ancien Locataire');
    expect(rows[1].querySelectorAll('td')[4].textContent.trim()).toBe('—');
    expect(rows[1].querySelectorAll('td')[5].textContent).toContain('à percevoir ce mois');
    expect(fixture.nativeElement.textContent).toContain('0 vacants');
    const occupancyMetric = fixture.nativeElement.querySelectorAll('ui-metric-card')[1];
    expect(occupancyMetric.textContent).toContain('100 %');
    expect(occupancyMetric.textContent).toContain('1 sur 1 lot actif');
    const totalMetric = fixture.nativeElement.querySelectorAll('ui-metric-card')[2];
    expect(totalMetric.textContent).toContain(currency(70000));
    expect(totalMetric.textContent).toContain('sans prorata');
    expect(fixture.nativeElement.querySelector('a[href="/lots/active"]')).toBeTruthy();
    rows[0].click();
    expect(navigateByUrl).toHaveBeenCalledWith('/lots/active');

    const archivedCheckbox = fixture.nativeElement.querySelector(
      'ui-lots-table input[type="checkbox"]',
    ) as HTMLInputElement;
    archivedCheckbox.checked = true;
    archivedCheckbox.dispatchEvent(new Event('change'));
    fixture.detectChanges();
    const expandedRows = fixture.nativeElement.querySelectorAll('ui-lots-table tbody tr');
    expect(expandedRows).toHaveLength(3);
    expect(expandedRows[2].textContent).toContain('Lot Z02');
  });

  it('should render an empty state when no lot exists', () => {
    const fixture = TestBed.createComponent(LotsPage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/lots').flush([]);
    http.expectOne('/api/leasing/lot-occupancies').flush([]);
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Ajoutez votre premier lot');
  });

  it('should use the complete current lease amount without applying a prorata', () => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date(2026, 6, 15));

    try {
      const fixture = TestBed.createComponent(LotsPage);
      fixture.detectChanges();
      const http = TestBed.inject(HttpTestingController);
      http.expectOne('/api/portfolio/lots').flush([createLot({ id: 'active' })]);
      http.expectOne('/api/leasing/lot-occupancies').flush([
        {
          lotId: 'active',
          leaseId: 'lease-1',
          tenantId: 'tenant-1',
          tenantFirstName: 'Camille',
          tenantLastName: 'Robert',
          startsOn: '2026-07-12',
          endsOn: null,
          rentDueDay: 5,
          rentExcludingChargesCents: 310000,
          chargesCents: 31000,
          depositCents: 310000,
          paymentTerms: null,
          notes: null,
        },
      ]);
      fixture.detectChanges();

      const row = fixture.nativeElement.querySelector('ui-lots-table tbody tr');
      expect(row.textContent).toContain('3 410,00 €');
      expect(row.textContent).toContain('3 100,00 €');
      expect(row.textContent).toContain('310,00 €');
      expect(fixture.nativeElement.textContent).toContain('Loyers et charges, sans prorata');
    } finally {
      vi.useRealTimers();
    }
  });

  it('should use only the current lease as the monthly potential reference', () => {
    vi.useFakeTimers();
    vi.setSystemTime(new Date(2026, 6, 15));

    try {
      const fixture = TestBed.createComponent(LotsPage);
      fixture.detectChanges();
      const http = TestBed.inject(HttpTestingController);
      http.expectOne('/api/portfolio/lots').flush([createLot({ id: 'active' })]);
      http.expectOne('/api/leasing/lot-occupancies').flush([
        {
          lotId: 'active',
          leaseId: 'lease-ended',
          tenantId: 'tenant-1',
          tenantFirstName: 'Camille',
          tenantLastName: 'Robert',
          startsOn: '2026-07-01',
          endsOn: '2026-07-11',
          rentDueDay: 5,
          rentExcludingChargesCents: 310000,
          chargesCents: 31000,
          depositCents: 310000,
          paymentTerms: null,
          notes: null,
        },
        {
          lotId: 'active',
          leaseId: 'lease-current',
          tenantId: 'tenant-2',
          tenantFirstName: 'Alex',
          tenantLastName: 'Martin',
          startsOn: '2026-07-12',
          endsOn: null,
          rentDueDay: 5,
          rentExcludingChargesCents: 620000,
          chargesCents: 62000,
          depositCents: 620000,
          paymentTerms: null,
          notes: null,
        },
      ]);
      fixture.detectChanges();

      const rows = fixture.nativeElement.querySelectorAll('ui-lots-table tbody tr');
      expect(rows).toHaveLength(1);
      expect(rows[0].textContent).toContain('Alex Martin');
      expect(rows[0].textContent).toContain('6 820,00 €');
      expect(rows[0].textContent).toContain('6 200,00 € HC');
      expect(rows[0].textContent).toContain('620,00 € charges');
    } finally {
      vi.useRealTimers();
    }
  });

  it('should use the configured potential for a vacant active lot', () => {
    const fixture = TestBed.createComponent(LotsPage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/lots').flush([
      createLot({
        id: 'vacant',
        potentialRentExcludingChargesCents: 81000,
        potentialChargesCents: 9000,
      }),
    ]);
    http.expectOne('/api/leasing/lot-occupancies').flush([]);
    fixture.detectChanges();

    const row = fixture.nativeElement.querySelector('ui-lots-table tbody tr');
    expect(row.textContent).toContain('900,00 €');
    expect(row.textContent).toContain('810,00 € HC');
    expect(row.textContent).toContain('90,00 € charges');
    const totalMetric = fixture.nativeElement.querySelectorAll('ui-metric-card')[2];
    expect(totalMetric.textContent).toContain(currency(90000));
  });
});

function createLot(overrides: Partial<Lot> = {}): Lot {
  return {
    id: 'lot-1',
    sciId: 'sci-1',
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
    ...overrides,
  };
}

function currentMonthDate(day: number): string {
  const currentDate = new Date();
  const year = currentDate.getFullYear();
  const month = String(currentDate.getMonth() + 1).padStart(2, '0');
  return `${year}-${month}-${String(day).padStart(2, '0')}`;
}

function currency(cents: number): string {
  return new Intl.NumberFormat('fr-FR', {
    style: 'currency',
    currency: 'EUR',
    minimumFractionDigits: 2,
    maximumFractionDigits: 2,
  }).format(cents / 100);
}
