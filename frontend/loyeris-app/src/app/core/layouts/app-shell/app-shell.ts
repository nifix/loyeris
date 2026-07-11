import { Component, computed, inject, input } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { take } from 'rxjs';

import { AuthSession } from '../../auth/auth-session';
import { BrandMark } from '../../../shared/components/ui-brand-mark/brand-mark';

type NavigationItem = {
  readonly label: string;
  readonly href: string;
  readonly icon: 'dashboard' | 'sci' | 'lots' | 'tenants' | 'rents' | 'settings' | 'logout';
  readonly separated?: boolean;
};

@Component({
  selector: 'ui-app-shell',
  imports: [BrandMark, RouterLink],
  templateUrl: './app-shell.html',
  styleUrl: './app-shell.css',
})
export class AppShell {
  private readonly authSession = inject(AuthSession);
  private readonly router = inject(Router);

  readonly activeItem = input<NavigationItem['icon']>('dashboard');
  readonly mobileActionLabel = input('Paiement');
  readonly mobileActionUrl = input('#');

  protected readonly navigation: readonly NavigationItem[] = [
    { label: 'Dashboard', href: '/dashboard', icon: 'dashboard' },
    { label: 'SCI', href: '/scis', icon: 'sci' },
    { label: 'Lots', href: '/lots', icon: 'lots' },
    { label: 'Locataires', href: '/tenants', icon: 'tenants' },
    { label: 'Loyers', href: '/rents', icon: 'rents' },
    { label: 'Paramètres', href: '/settings', icon: 'settings' },
    { label: 'Déconnexion', href: '#', icon: 'logout', separated: true },
  ];

  protected readonly userName = computed(() => {
    const user = this.authSession.user();
    return user ? `${user.firstName} ${user.lastName}`.trim() : '';
  });
  protected readonly userEmail = computed(() => this.authSession.user()?.email ?? '');
  protected readonly userInitials = computed(() => {
    const user = this.authSession.user();
    return user
      ? `${user.firstName?.[0] ?? ''}${user.lastName?.[0] ?? ''}`.toUpperCase()
      : '';
  });

  protected logout(): void {
    this.authSession
      .logout()
      .pipe(take(1))
      .subscribe({
        next: () => void this.router.navigate(['/login']),
        error: () => void this.router.navigate(['/login']),
      });
  }
}
