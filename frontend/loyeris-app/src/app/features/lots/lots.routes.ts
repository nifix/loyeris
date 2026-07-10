import { Routes } from '@angular/router';
import { LotDetailPage } from './pages/lot-detail-page/lot-detail-page';
import { LotsPage } from './pages/lots-page/lots-page';

export const lotsRoutes: Routes = [
  {
    path: '',
    component: LotsPage,
  },
  {
    path: ':lotId',
    component: LotDetailPage,
  },
];
