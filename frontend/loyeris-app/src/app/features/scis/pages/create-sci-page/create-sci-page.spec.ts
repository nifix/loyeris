import { provideHttpClient } from '@angular/common/http';
import { HttpTestingController, provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter, Router } from '@angular/router';

import { type Sci } from '../../services/sci-api';
import { CreateSciPage } from './create-sci-page';

describe('CreateSciPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [CreateSciPage],
      providers: [provideHttpClient(), provideHttpClientTesting(), provideRouter([])],
    }).compileComponents();
  });

  it('should reject an invalid form without calling the API', () => {
    const fixture = TestBed.createComponent(CreateSciPage);
    fixture.detectChanges();
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Le nom de la SCI est obligatoire');
    TestBed.inject(HttpTestingController).expectNone('/api/portfolio/scis');
  });

  it('should create a SCI with the expected payload and navigate to the list', async () => {
    const fixture = TestBed.createComponent(CreateSciPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#sci-name', ' SCI Les Tilleuls ');
    setInputValue(fixture.nativeElement, '#sci-siren', '123456789');
    setInputValue(fixture.nativeElement, '#sci-street', ' 12 rue des Tilleuls ');
    setInputValue(fixture.nativeElement, '#sci-postal-code', '69000');
    setInputValue(fixture.nativeElement, '#sci-city', ' Lyon ');

    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    const request = TestBed.inject(HttpTestingController).expectOne('/api/portfolio/scis');
    expect(request.request.method).toBe('POST');
    expect(request.request.body).toEqual({
      name: 'SCI Les Tilleuls',
      siren: '123456789',
      taxRegime: 'IR',
      status: 'Active',
      street: '12 rue des Tilleuls',
      postalCode: '69000',
      city: 'Lyon',
      country: 'FR',
      incorporatedOn: null,
    });
    request.flush({ id: 'sci-id' });
    await fixture.whenStable();

    expect(navigate).toHaveBeenCalledWith(['/scis'], { queryParams: { created: '1' } });
  });

  it('should prevent duplicate submissions while the request is pending', () => {
    const fixture = TestBed.createComponent(CreateSciPage);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#sci-name', 'SCI Test');
    const form = fixture.nativeElement.querySelector('form') as HTMLFormElement;

    form.dispatchEvent(new Event('submit'));
    form.dispatchEvent(new Event('submit'));

    TestBed.inject(HttpTestingController).expectOne('/api/portfolio/scis');
  });

  it('should display a stable duplicate-SIREN error', () => {
    const fixture = TestBed.createComponent(CreateSciPage);
    fixture.detectChanges();
    setInputValue(fixture.nativeElement, '#sci-name', 'SCI Test');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));
    TestBed.inject(HttpTestingController)
      .expectOne('/api/portfolio/scis')
      .flush(
        { errorCode: 'portfolio.sci.siren_already_exists' },
        { status: 409, statusText: 'Conflict' },
      );
    fixture.detectChanges();

    expect(fixture.nativeElement.textContent).toContain('Ce numéro SIREN est déjà utilisé');
  });

  it('should populate the shared form and update an existing SCI', async () => {
    const fixture = TestBed.createComponent(CreateSciPage);
    const router = TestBed.inject(Router);
    const navigate = vi.spyOn(router, 'navigate').mockResolvedValue(true);
    fixture.componentRef.setInput('editingSci', createSci());
    fixture.detectChanges();

    const nameInput = fixture.nativeElement.querySelector('#sci-name') as HTMLInputElement;
    expect(nameInput.value).toBe('SCI Les Tilleuls');
    expect(fixture.nativeElement.textContent).toContain('Modifier la SCI');

    setInputValue(fixture.nativeElement, '#sci-name', ' SCI Les Tilleuls Patrimoine ');
    fixture.nativeElement.querySelector('form').dispatchEvent(new Event('submit'));

    const request = TestBed.inject(HttpTestingController).expectOne('/api/portfolio/scis/sci-1');
    expect(request.request.method).toBe('PUT');
    expect(request.request.body.name).toBe('SCI Les Tilleuls Patrimoine');
    request.flush(createSci({ name: 'SCI Les Tilleuls Patrimoine' }));
    await fixture.whenStable();

    expect(navigate).toHaveBeenCalledWith(['/scis'], { queryParams: { updated: '1' } });
  });
});

function createSci(overrides: Partial<Sci> = {}): Sci {
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
    ...overrides,
  };
}

function setInputValue(element: HTMLElement, selector: string, value: string): void {
  const input = element.querySelector(selector) as HTMLInputElement;
  input.value = value;
  input.dispatchEvent(new Event('input'));
}
