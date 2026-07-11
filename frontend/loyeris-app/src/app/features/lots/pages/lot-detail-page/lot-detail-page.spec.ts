import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LotDetailPage } from './lot-detail-page';

describe('LotDetailPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LotDetailPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create the lot detail page', () => {
    const fixture = TestBed.createComponent(LotDetailPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display lot, occupation and payment information', () => {
    const fixture = TestBed.createComponent(LotDetailPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Lot A02');
    expect(compiled.textContent).toContain('24 m²');
    expect(compiled.textContent).toContain('Camille Robert');
    expect(compiled.textContent).toContain('Historique des paiements');
  });
});
