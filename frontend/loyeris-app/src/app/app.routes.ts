import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'login',
  },
  {
    path: 'login',
    loadChildren: () => import('./features/login/login.routes').then((m) => m.loginRoutes),
  },
  {
    path: 'register',
    loadChildren: () => import('./features/login/login.routes').then((m) => m.registerRoutes),
  },
  {
    path: 'verify-email',
    loadChildren: () =>
      import('./features/login/login.routes').then((m) => m.emailVerificationRoutes),
  },
  {
    path: 'dashboard',
    loadChildren: () => import('./features/dashboard/dashboard.routes').then((m) => m.dashboardRoutes),
  },
  {
    path: 'scis',
    loadChildren: () => import('./features/scis/scis.routes').then((m) => m.scisRoutes),
  },
  {
    path: 'lots',
    loadChildren: () => import('./features/lots/lots.routes').then((m) => m.lotsRoutes),
  },
  {
    path: 'tenants',
    loadChildren: () => import('./features/tenants/tenants.routes').then((m) => m.tenantsRoutes),
  },
  {
    path: 'rents',
    loadChildren: () => import('./features/rents/rents.routes').then((m) => m.rentsRoutes),
  },
  {
    path: 'settings',
    loadChildren: () => import('./features/settings/settings.routes').then((m) => m.settingsRoutes),
  },
];
