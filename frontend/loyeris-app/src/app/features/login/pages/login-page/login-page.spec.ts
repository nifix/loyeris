import { TestBed } from '@angular/core/testing';
import { LoginPage } from './login-page';

describe('LoginPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [LoginPage] }).compileComponents();
  });

  it('should create the login page', () => {
    const fixture = TestBed.createComponent(LoginPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render the complete login value proposition and form', () => {
    const fixture = TestBed.createComponent(LoginPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Le cockpit locatif pensé pour remplacer Excel');
    expect(compiled.textContent).toContain('Pages déjà maquettées');
    expect(compiled.querySelector('input[type="email"]')).toBeTruthy();
    expect(compiled.querySelector('input[type="password"]')).toBeTruthy();
  });
});
