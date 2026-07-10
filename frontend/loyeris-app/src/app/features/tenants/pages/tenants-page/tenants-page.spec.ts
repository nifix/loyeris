import { TestBed } from '@angular/core/testing';
import { TenantsPage } from './tenants-page';

describe('TenantsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [TenantsPage] }).compileComponents();
  });

  it('should create the tenants page', () => {
    const fixture = TestBed.createComponent(TenantsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render four tenants and their portfolio summary', () => {
    const fixture = TestBed.createComponent(TenantsPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('3 locataires actifs');
    expect(compiled.textContent).toContain('Camille Robert');
    expect(compiled.textContent).toContain('Élodie Marchal');
    expect(compiled.querySelectorAll('ui-tenants-table tbody tr')).toHaveLength(4);
  });
});
