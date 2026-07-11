import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { DashboardPage } from './dashboard-page';

describe('DashboardPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DashboardPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create the dashboard page', () => {
    const fixture = TestBed.createComponent(DashboardPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the dashboard metrics and operational cards', () => {
    const fixture = TestBed.createComponent(DashboardPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelectorAll('ui-metric-card')).toHaveLength(4);
    expect(compiled.textContent).toContain('Loyers en retard');
    expect(compiled.textContent).toContain('Paiements récents');
    expect(compiled.textContent).toContain('Occupation des lots');
  });
});
