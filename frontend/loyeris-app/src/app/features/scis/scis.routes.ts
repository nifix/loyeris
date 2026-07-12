import { Routes } from '@angular/router';
import { ScisPage } from './pages/scis-page/scis-page';
import { CreateSciPage } from './pages/create-sci-page/create-sci-page';

export const scisRoutes: Routes = [
  {
    path: 'new',
    component: CreateSciPage,
  },
  {
    path: ':sciId/edit',
    component: CreateSciPage,
  },
  {
    path: '',
    component: ScisPage,
  },
];
