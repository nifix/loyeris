import { Component } from '@angular/core';
import { BrandMark } from '../../../shared/components/ui-brand-mark/brand-mark';

type NavigationItem = {
  readonly label: string;
  readonly href: string;
  readonly icon: 'dashboard' | 'sci' | 'lots' | 'tenants' | 'rents' | 'settings' | 'logout';
  readonly active?: boolean;
  readonly separated?: boolean;
};

@Component({
  selector: 'ui-app-shell',
  imports: [BrandMark],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.css',
})
export class AppShell {
  protected readonly navigation: readonly NavigationItem[] = [
    { label: 'Dashboard', href: '/dashboard', icon: 'dashboard', active: true },
    { label: 'SCI', href: '#', icon: 'sci' },
    { label: 'Lots', href: '#', icon: 'lots' },
    { label: 'Locataires', href: '#', icon: 'tenants' },
    { label: 'Loyers', href: '#', icon: 'rents' },
    { label: 'Paramètres', href: '#', icon: 'settings' },
    { label: 'Déconnexion', href: '#', icon: 'logout', separated: true },
  ];
}
