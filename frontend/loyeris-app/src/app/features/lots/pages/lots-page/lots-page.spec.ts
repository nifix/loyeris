import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { LotsPage } from './lots-page';

describe('LotsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [LotsPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create the lots page', () => {
    const fixture = TestBed.createComponent(LotsPage);

    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display the portfolio summary and the six reference lots', () => {
    const fixture = TestBed.createComponent(LotsPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const rows = compiled.querySelectorAll('ui-lots-table tbody tr');

    expect(compiled.textContent).toContain('9 lots suivis');
    expect(compiled.textContent).toContain('Lot A01');
    expect(compiled.textContent).toContain('Lot C01');
    expect(compiled.textContent).toContain('Vacant');
    expect(rows).toHaveLength(6);
  });
});
