import { Routes } from '@angular/router';
import { TenantDetailPage } from './pages/tenant-detail-page/tenant-detail-page';
import { TenantsPage } from './pages/tenants-page/tenants-page';

export const tenantsRoutes: Routes = [
  {
    path: '',
    component: TenantsPage,
  },
  {
    path: ':tenantId',
    component: TenantDetailPage,
  },
];
