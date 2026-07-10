import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { SciCard } from '../../components/ui-sci-card/sci-card';

@Component({
  selector: 'app-scis-page',
  imports: [AppShell, MetricCard, PageHeader, SciCard],
  templateUrl: './scis-page.html',
  styleUrl: './scis-page.css',
})
export class ScisPage {
  protected readonly headerChips = [
    '2 structures suivies',
    '4 320 € de loyers mensuels',
    "78% d'occupation moyenne",
  ];

  protected readonly metrics = [
    {
      title: 'Structures actives',
      value: '2',
      description: 'Toutes en production',
      tone: 'primary' as const,
    },
    {
      title: 'Loyers mensuels',
      value: '4 320 €',
      description: 'Potentiel consolidé',
      tone: 'neutral' as const,
    },
    {
      title: 'Occupation moyenne',
      value: '78%',
      description: 'Sur 9 lots',
      tone: 'success' as const,
    },
  ];

  protected readonly scis = [
    {
      name: 'SCI Les Tilleuls',
      address: '12 rue des Tilleuls, Lyon',
      createdAt: '14 janvier 2024',
      lots: 5,
      tenants: 4,
      monthlyRent: '2 760 €',
      occupancy: '80%',
      performance: 79,
      performanceTone: 'primary' as const,
    },
    {
      name: 'SCI Carnot',
      address: '8 boulevard Carnot, Villeurbanne',
      createdAt: '3 mars 2025',
      lots: 4,
      tenants: 3,
      monthlyRent: '1 560 €',
      occupancy: '75%',
      performance: 87,
      performanceTone: 'success' as const,
    },
  ];
}
