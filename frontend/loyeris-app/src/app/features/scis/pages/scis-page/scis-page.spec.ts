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
    const request = TestBed.inject(HttpTestingController).expectOne('/api/portfolio/scis');
    expect(request.request.method).toBe('GET');
    request.flush([
      createSci({ id: 'sci-2', name: 'SCI Carnot', status: 'Archived', siren: null }),
      createSci({ id: 'sci-1', name: 'SCI Les Tilleuls', status: 'Active' }),
    ]);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const cards = compiled.querySelectorAll('ui-sci-card');

    expect(compiled.textContent).toContain('2 structures suivies');
    expect(compiled.textContent).not.toContain('Workspace courant');
    expect(compiled.textContent).toContain('SCI Les Tilleuls');
    expect(compiled.textContent).toContain('SCI Carnot');
    expect(compiled.textContent).toContain('10/01/2024');
    expect(cards).toHaveLength(2);
    expect(cards[0].textContent).toContain('SCI Les Tilleuls');
    expect(cards[1].textContent).toContain('SCI Carnot');
    expect(compiled.querySelector('a[href="/scis/sci-1/edit"]')).toBeTruthy();
    expect(compiled.textContent).not.toContain('4 320 €');
  });

  it('should render an empty state when the workspace has no SCI', () => {
    const fixture = TestBed.createComponent(ScisPage);
    fixture.detectChanges();
    TestBed.inject(HttpTestingController).expectOne('/api/portfolio/scis').flush([]);
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
