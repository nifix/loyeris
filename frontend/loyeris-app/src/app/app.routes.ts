import { Routes } from '@angular/router';
import { authGuard, guestGuard } from './core/auth/auth-guards';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'login',
    canActivate: [guestGuard],
    loadChildren: () => import('./features/login/login.routes').then((m) => m.loginRoutes),
  },
  {
    path: 'register',
    canActivate: [guestGuard],
    loadChildren: () => import('./features/login/login.routes').then((m) => m.registerRoutes),
  },
  {
    path: 'verify-email',
    loadChildren: () =>
      import('./features/login/login.routes').then((m) => m.emailVerificationRoutes),
  },
  {
    path: 'dashboard',
    canActivate: [authGuard],
    loadChildren: () => import('./features/dashboard/dashboard.routes').then((m) => m.dashboardRoutes),
  },
  {
    path: 'scis',
    canActivate: [authGuard],
    loadChildren: () => import('./features/scis/scis.routes').then((m) => m.scisRoutes),
  },
  {
    path: 'lots',
    canActivate: [authGuard],
    loadChildren: () => import('./features/lots/lots.routes').then((m) => m.lotsRoutes),
  },
  {
    path: 'tenants',
    canActivate: [authGuard],
    loadChildren: () => import('./features/tenants/tenants.routes').then((m) => m.tenantsRoutes),
  },
  {
    path: 'rents',
    canActivate: [authGuard],
    loadChildren: () => import('./features/rents/rents.routes').then((m) => m.rentsRoutes),
  },
  {
    path: 'settings',
    canActivate: [authGuard],
    loadChildren: () => import('./features/settings/settings.routes').then((m) => m.settingsRoutes),
  },
];
