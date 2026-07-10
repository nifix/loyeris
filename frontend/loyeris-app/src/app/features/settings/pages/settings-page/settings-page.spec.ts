import { TestBed } from '@angular/core/testing';
import { SettingsPage } from './settings-page';

describe('SettingsPage', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({ imports: [SettingsPage] }).compileComponents();
  });

  it('should create the settings page', () => {
    const fixture = TestBed.createComponent(SettingsPage);
    expect(fixture.componentInstance).toBeTruthy();
  });

  it('should render all four settings sections and account fields', () => {
    const fixture = TestBed.createComponent(SettingsPage);
    fixture.detectChanges();

    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.textContent).toContain('Profil');
    expect(compiled.textContent).toContain('Sécurité');
    expect(compiled.textContent).toContain('Préférences');
    expect(compiled.textContent).toContain('Apparence');
    expect(compiled.querySelectorAll('ui-settings-panel')).toHaveLength(4);
  });
});
