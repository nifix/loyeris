import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { LotFormPage } from './lot-form-page';

describe('LotFormPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LotFormPage],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();
  });

  it('should create a lot and its optional active occupancy', async () => {
    const fixture = TestBed.createComponent(LotFormPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/scis').flush([createSci()]);
    http.expectOne('/api/leasing/available-tenants').flush([
      {
        id: 'tenant-1',
        firstName: 'Camille',
        lastName: 'Robert',
        email: 'camille@example.fr',
        isCurrent: false,
      },
    ]);
    fixture.detectChanges();

    setValue(fixture.nativeElement, '#lot-reference', ' Lot A01 ');
    setValue(fixture.nativeElement, '#lot-street', ' 12 rue des Tilleuls ');
    setValue(fixture.nativeElement, '#lot-postal-code', '69000');
    setValue(fixture.nativeElement, '#lot-city', ' Lyon ');
    setValue(fixture.nativeElement, '#lot-type', 'T5', 'change');
    setValue(fixture.nativeElement, '#lot-potential-rent', '650');
    findButton(fixture.nativeElement, 'Assigner un locataire').click();
    fixture.detectChanges();
    setValue(fixture.nativeElement, '#lot-tenant', 'tenant-1', 'change');
    fixture.detectChanges();
    setValue(fixture.nativeElement, '#lease-ends-on', '2027-06-30');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    const lotRequest = http.expectOne('/api/portfolio/lots');
    expect(lotRequest.request.method).toBe('POST');
    expect(lotRequest.request.body).toMatchObject({
      sciId: 'sci-1',
      reference: 'Lot A01',
      type: 'T5',
      potentialRentExcludingChargesCents: 65000,
    });
    lotRequest.flush(createLot());

    const occupancyRequest = http.expectOne('/api/leasing/lots/lot-1/occupancy');
    expect(occupancyRequest.request.method).toBe('PUT');
    expect(occupancyRequest.request.body).toMatchObject({
      tenantId: 'tenant-1',
      endsOn: '2027-06-30',
      rentExcludingChargesCents: 65000,
      rentDueDay: 5,
    });
    occupancyRequest.flush(null);
    await fixture.whenStable();

    expect(navigate).toHaveBeenCalledWith(['/lots'], { queryParams: { created: '1' } });
  });

  it('should reject incomplete lot data without creating a lot', () => {
    const fixture = TestBed.createComponent(LotFormPage);
    fixture.detectChanges();
    const http = TestBed.inject(HttpTestingController);
    http.expectOne('/api/portfolio/scis').flush([createSci()]);
    http.expectOne('/api/leasing/available-tenants').flush([]);
    fixture.detectChanges();

    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    http.expectNone('/api/portfolio/lots');
    expect(fixture.nativeElement.textContent).toContain('La référence est obligatoire');
  });

  it('should save an existing lease independently from the lot', async () => {
    const fixture = TestBed.createComponent(LotFormPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.componentRef.setInput('presentationLot', createLot());
    fixture.componentRef.setInput('presentationScis', [createSci()]);
    fixture.componentRef.setInput('presentationTenants', [{
      id: 'tenant-1',
      firstName: 'Camille',
      lastName: 'Robert',
      email: 'camille@example.fr',
      isCurrent: true,
    }]);
    fixture.componentRef.setInput('presentationOccupancy', createOccupancy());
    fixture.detectChanges();

    expect((fixture.nativeElement.querySelector('#lot-reference') as HTMLInputElement).value)
      .toBe('Lot A01');
    expect((fixture.nativeElement.querySelector('#lot-reference') as HTMLInputElement).disabled)
      .toBe(true);
    expect(fixture.nativeElement.textContent).toContain('Détails du lot');
    findButton(fixture.nativeElement, 'Modifier le lot').click();
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Camille Robert');
    expect(fixture.nativeElement.textContent).toContain('01/07/2026 — 30/06/2027');
    findButton(fixture.nativeElement, 'Modifier').click();
    fixture.detectChanges();
    expect(fixture.nativeElement.textContent).toContain('Locataire du bail');
    const leaseCard = fixture.nativeElement.querySelector('[aria-label="Derniers baux du lot"] article');
    expect(leaseCard.nextElementSibling?.textContent).toContain('Modification du bail');
    setValue(fixture.nativeElement, '#lease-rent', '700');
    findButton(fixture.nativeElement, 'Sauvegarder le bail').click();

    const http = TestBed.inject(HttpTestingController);
    const leaseRequest = http.expectOne('/api/leasing/lots/lot-1/leases/lease-1');
    expect(leaseRequest.request.method).toBe('PUT');
    expect(leaseRequest.request.body).not.toHaveProperty('tenantId');
    expect(leaseRequest.request.body.rentExcludingChargesCents).toBe(70000);
    leaseRequest.flush({ ...createOccupancy(), rentExcludingChargesCents: 70000 });
    http.expectOne('/api/leasing/lots/lot-1/leases').flush([
      { ...createOccupancy(), rentExcludingChargesCents: 70000 },
    ]);
    http.expectOne('/api/leasing/available-tenants?lotId=lot-1').flush([]);
    await fixture.whenStable();

    expect(navigate).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Le bail a bien été enregistré');

    setValue(fixture.nativeElement, '#lot-reference', 'Lot A01 rénové');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    const lotRequest = http.expectOne('/api/portfolio/lots/lot-1');
    expect(lotRequest.request.method).toBe('PUT');
    lotRequest.flush(createLot());
    await fixture.whenStable();
    expect(navigate).not.toHaveBeenCalled();
    expect(fixture.nativeElement.textContent).toContain('Les informations du lot ont bien été enregistrées');
    expect((fixture.nativeElement.querySelector('#lot-reference') as HTMLInputElement).disabled)
      .toBe(true);
  });

  it('should require confirmation before deleting a lease', async () => {
    const fixture = TestBed.createComponent(LotFormPage);
    fixture.componentRef.setInput('presentationLot', createLot());
    fixture.componentRef.setInput('presentationScis', [createSci()]);
    fixture.componentRef.setInput('presentationTenants', []);
    fixture.componentRef.setInput('presentationOccupancy', createOccupancy());
    fixture.detectChanges();

    findButton(fixture.nativeElement, 'Modifier le lot').click();
    fixture.detectChanges();
    findButton(fixture.nativeElement, 'Modifier').click();
    fixture.detectChanges();
    findButton(fixture.nativeElement, 'Supprimer le bail').click();
    fixture.detectChanges();

    const http = TestBed.inject(HttpTestingController);
    http.expectNone('/api/leasing/lots/lot-1/leases/lease-1');
    expect(fixture.nativeElement.textContent).toContain('Supprimer ce bail ?');

    findButton(fixture.nativeElement, 'Confirmer la suppression').click();
    const deleteRequest = http.expectOne('/api/leasing/lots/lot-1/leases/lease-1');
    expect(deleteRequest.request.method).toBe('DELETE');
    deleteRequest.flush(null);
    http.expectOne('/api/leasing/lots/lot-1/leases').flush([]);
    http.expectOne('/api/leasing/available-tenants?lotId=lot-1').flush([]);
    await fixture.whenStable();

    expect(fixture.nativeElement.textContent).toContain('Le bail a bien été supprimé');
    expect(fixture.nativeElement.textContent).toContain('Aucun bail enregistré');
  });
});

function setValue(
  element: HTMLElement,
  selector: string,
  value: string,
  eventName = 'input',
): void {
  const control = element.querySelector(selector) as HTMLInputElement | HTMLSelectElement;
  control.value = value;
  control.dispatchEvent(new Event(eventName));
}

function findButton(element: HTMLElement, label: string): HTMLButtonElement {
  return [...element.querySelectorAll('button')]
    .find((button) => button.textContent?.includes(label)) as HTMLButtonElement;
}

function createSci() {
  return {
    id: 'sci-1',
    workspaceId: 'workspace-1',
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
}

function createLot() {
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
    surfaceSqm: null,
    potentialRentExcludingChargesCents: 65000,
    potentialChargesCents: 0,
    suggestedDepositCents: 0,
    notes: null,
    createdAt: '2026-07-12T08:00:00Z',
    updatedAt: '2026-07-12T08:00:00Z',
    archivedAt: null,
  };
}

function createOccupancy() {
  return {
    lotId: 'lot-1',
    leaseId: 'lease-1',
    tenantId: 'tenant-1',
    tenantFirstName: 'Camille',
    tenantLastName: 'Robert',
    startsOn: '2026-07-01',
    endsOn: '2027-06-30',
    rentDueDay: 5,
    rentExcludingChargesCents: 65000,
    chargesCents: 5000,
    depositCents: 65000,
    paymentTerms: null,
    notes: null,
  };
}
