import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import {
  TenantsTable,
  type TenantListItem,
} from '../../components/ui-tenants-table/tenants-table';

@Component({
  selector: 'app-tenants-page',
  imports: [AppShell, MetricCard, PageHeader, TenantsTable],
  templateUrl: './tenants-page.html',
  styleUrl: './tenants-page.css',
})
export class TenantsPage {
  protected readonly headerChips = [
    '3 locataires actifs',
    '1 sortie archivée',
    '1 retard à surveiller',
  ];

  protected readonly metrics = [
    {
      title: 'Locataires actifs',
      value: '3',
      description: '1 locataire sorti',
      tone: 'success' as const,
    },
    {
      title: 'Entrées récentes',
      value: '1',
      description: 'Depuis février 2026',
      tone: 'primary' as const,
    },
    {
      title: 'Retards en cours',
      value: '1',
      description: 'Camille Robert',
      tone: 'warning' as const,
    },
  ];

  protected readonly tenants: readonly TenantListItem[] = [
    {
      name: 'Sophie Martin',
      sci: 'SCI Les Tilleuls',
      currentLot: 'Lot A01',
      entryDate: '15/09/2025',
      email: 'sophie.martin@email.fr',
      phone: '06 11 22 33 44',
      status: 'Actif',
      statusTone: 'success',
      detailUrl: '/tenants/sophie-martin',
    },
    {
      name: 'Camille Robert',
      sci: 'SCI Les Tilleuls',
      currentLot: 'Lot A02',
      entryDate: '01/02/2026',
      email: 'camille.robert@email.fr',
      phone: '06 22 33 44 55',
      status: 'Actif',
      statusTone: 'success',
      detailUrl: '/tenants/camille-robert',
    },
    {
      name: 'Louis Mercier',
      sci: 'SCI Carnot',
      currentLot: 'Lot B01',
      entryDate: '01/10/2025',
      email: 'louis.mercier@email.fr',
      phone: '06 33 44 55 66',
      status: 'Actif',
      statusTone: 'success',
      detailUrl: '/tenants/louis-mercier',
    },
    {
      name: 'Élodie Marchal',
      sci: 'SCI Les Tilleuls',
      currentLot: '—',
      entryDate: '01/05/2024',
      email: 'elodie.marchal@email.fr',
      phone: '06 44 55 66 77',
      status: 'Parti',
      statusTone: 'neutral',
      detailUrl: '/tenants/elodie-marchal',
    },
  ];
}
