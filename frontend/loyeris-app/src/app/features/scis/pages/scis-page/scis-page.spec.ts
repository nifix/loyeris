import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { ScisPage } from './scis-page';

describe('ScisPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ScisPage],
      providers: [provideRouter([])],
    }).compileComponents();
  });

  it('should create the SCI portfolio page', () => {
    const fixture = TestBed.createComponent(ScisPage);

    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should display the portfolio summary and both SCI cards', () => {
    const fixture = TestBed.createComponent(ScisPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    const cards = compiled.querySelectorAll('ui-sci-card');

    expect(compiled.textContent).toContain('2 structures suivies');
    expect(compiled.textContent).toContain('SCI Les Tilleuls');
    expect(compiled.textContent).toContain('SCI Carnot');
    expect(cards).toHaveLength(2);
  });
});
