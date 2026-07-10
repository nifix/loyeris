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
];
