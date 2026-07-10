import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { DashboardCard } from '../../components/ui-dashboard-card/dashboard-card';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

@Component({
  selector: 'app-dashboard-page',
  imports: [AppShell, DashboardCard, MetricCard, PageHeader, StatusBadge],
  templateUrl: './dashboard-page.html',
  styleUrl: './dashboard-page.css',
})
export class DashboardPage {
  protected readonly headerChips = ['2 SCI actives', '7 lots occupés', '3 actions prioritaires'];

  protected readonly metrics = [
    {
      title: 'Loyers attendus',
      value: '4 320 €',
      description: 'Avril 2026',
      tone: 'primary' as const,
    },
    {
      title: 'Loyers encaissés',
      value: '3 540 €',
      description: '82% du total attendu',
      tone: 'success' as const,
    },
    {
      title: 'Reste à recevoir',
      value: '780 €',
      description: '3 paiements à compléter',
      tone: 'warning' as const,
    },
    {
      title: 'Lots occupés',
      value: '7 / 9',
      description: "Taux d'occupation : 77%",
      tone: 'neutral' as const,
    },
  ];

  protected readonly overdueRents = [
    {
      lot: 'Lot A02',
      tenant: 'Camille Robert',
      sci: 'SCI Les Tilleuls',
      amount: '620 €',
      status: 'En retard',
      tone: 'error' as const,
      action: 'Relancer',
      actionTone: 'error' as const,
      actionUrl: '/rents',
    },
    {
      lot: 'Lot B01',
      tenant: 'Louis Mercier',
      sci: 'SCI Carnot',
      amount: '160 €',
      status: 'Partiel',
      tone: 'warning' as const,
      action: 'Compléter',
      actionTone: 'warning' as const,
      actionUrl: '/rents',
    },
    {
      lot: 'Lot C03',
      tenant: 'Anna Pierre',
      sci: 'SCI Les Tilleuls',
      amount: '0 €',
      status: 'Payé',
      tone: 'success' as const,
      action: 'Détail →',
      actionTone: 'primary' as const,
      actionUrl: '/tenants/camille-robert',
    },
  ];

  protected readonly recentPayments = [
    { tenant: 'Sophie Martin', details: 'Lot A01 · 26 avril 2026', amount: '680 €' },
    { tenant: 'Mathieu Leroy', details: 'Lot B03 · 24 avril 2026', amount: '540 €' },
    { tenant: 'Julie Lambert', details: 'Lot C01 · 23 avril 2026', amount: '720 €' },
    { tenant: 'Anna Pierre', details: 'Lot C03 · 22 avril 2026', amount: '580 €' },
  ];

  protected readonly sciBreakdown = [
    {
      name: 'SCI Les Tilleuls',
      amounts: '2 180 € / 2 760 €',
      progress: 79,
      tone: 'primary' as const,
    },
    {
      name: 'SCI Carnot',
      amounts: '1 360 € / 1 560 €',
      progress: 87,
      tone: 'success' as const,
    },
  ];
}
