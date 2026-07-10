import { TestBed } from '@angular/core/testing';
import { RentsPage } from './rents-page';

describe('RentsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [RentsPage] }).compileComponents();
  });

  it('should create the rents page', () => {
    const fixture = TestBed.createComponent(RentsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the monthly summary and five rent deadlines', () => {
    const fixture = TestBed.createComponent(RentsPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('4 320 € attendus');
    expect(compiled.textContent).toContain('Deux actions prioritaires');
    expect(compiled.textContent).toContain('En retard');
    expect(compiled.querySelectorAll('ui-rents-table tbody tr')).toHaveLength(5);
  });
});
