import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { LotsTable, type LotListItem } from '../../components/ui-lots-table/lots-table';

@Component({
  selector: 'app-lots-page',
  imports: [AppShell, LotsTable, MetricCard, PageHeader],
  templateUrl: './lots-page.html',
  styleUrl: './lots-page.css',
})
export class LotsPage {
  protected readonly headerChips = [
    '9 lots suivis',
    '7 occupés',
    '5 080 € de potentiel mensuel',
  ];

  protected readonly metrics = [
    {
      title: 'Lots gérés',
      value: '9',
      description: 'Sur 2 SCI',
      tone: 'neutral' as const,
    },
    {
      title: 'Occupés',
      value: '7',
      description: '2 lots vacants',
      tone: 'success' as const,
    },
    {
      title: 'Loyer potentiel',
      value: '5 080 €',
      description: 'Mensuel',
      tone: 'primary' as const,
    },
  ];

  protected readonly lots: readonly LotListItem[] = [
    {
      reference: 'Lot A01',
      sci: 'SCI Les Tilleuls',
      address: '12 rue des Tilleuls, Lyon',
      type: 'T2',
      occupation: 'Sophie Martin',
      rent: '680 €',
      status: 'Occupé',
      statusTone: 'success',
      detailUrl: '#',
    },
    {
      reference: 'Lot A02',
      sci: 'SCI Les Tilleuls',
      address: '12 rue des Tilleuls, Lyon',
      type: 'Studio',
      occupation: 'Camille Robert',
      rent: '620 €',
      status: 'Occupé',
      statusTone: 'success',
      detailUrl: '#',
    },
    {
      reference: 'Lot A03',
      sci: 'SCI Les Tilleuls',
      address: '12 rue des Tilleuls, Lyon',
      type: 'Garage',
      occupation: '—',
      rent: '90 €',
      status: 'Vacant',
      statusTone: 'warning',
      detailUrl: '#',
    },
    {
      reference: 'Lot B01',
      sci: 'SCI Carnot',
      address: '8 boulevard Carnot, Villeurbanne',
      type: 'T1',
      occupation: 'Louis Mercier',
      rent: '540 €',
      status: 'Occupé',
      statusTone: 'success',
      detailUrl: '#',
    },
    {
      reference: 'Lot B03',
      sci: 'SCI Carnot',
      address: '8 boulevard Carnot, Villeurbanne',
      type: 'T2',
      occupation: 'Mathieu Leroy',
      rent: '720 €',
      status: 'Occupé',
      statusTone: 'success',
      detailUrl: '#',
    },
    {
      reference: 'Lot C01',
      sci: 'SCI Les Tilleuls',
      address: '4 impasse des Peupliers, Lyon',
      type: 'T3',
      occupation: 'Julie Lambert',
      rent: '950 €',
      status: 'Occupé',
      statusTone: 'success',
      detailUrl: '#',
    },
  ];
}
