import { Routes } from '@angular/router';
import { LotFormPage } from './pages/lot-form-page/lot-form-page';
import { LotsPage } from './pages/lots-page/lots-page';

export const lotsRoutes: Routes = [
  {
    path: 'new',
    component: LotFormPage,
  },
  {
    path: ':lotId/edit',
    component: LotFormPage,
  },
  {
    path: '',
    component: LotsPage,
  },
  {
    path: ':lotId',
    component: LotFormPage,
  },
];
