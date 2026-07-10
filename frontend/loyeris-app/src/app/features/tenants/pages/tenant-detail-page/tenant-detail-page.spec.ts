import { TestBed } from '@angular/core/testing';
import { TenantDetailPage } from './tenant-detail-page';

describe('TenantDetailPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [TenantDetailPage] }).compileComponents();
  });

  it('should create the tenant detail page', () => {
    const fixture = TestBed.createComponent(TenantDetailPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display Camille Robert contact, rental and payment history', () => {
    const fixture = TestBed.createComponent(TenantDetailPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Camille Robert');
    expect(compiled.textContent).toContain('camille.robert@email.fr');
    expect(compiled.textContent).toContain('Historique locatif');
    expect(compiled.textContent).toContain('Avril 2026');
  });
});
