import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { DetailCard } from '../../../../shared/components/ui-detail-card/detail-card';
import { DetailPageHeader } from '../../../../shared/components/ui-detail-page-header/detail-page-header';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

@Component({
  selector: 'app-lot-detail-page',
  imports: [AppShell, DetailCard, DetailPageHeader, RouterLink, StatusBadge],
  templateUrl: './lot-detail-page.html',
  styleUrl: './lot-detail-page.css',
})
export class LotDetailPage {
  protected readonly headerChips = [
    '24 m²',
    '620,00 € charges comprises',
    'Prochaine échéance le 5 mai',
  ];

  protected readonly occupationHistory = [
    { tenant: 'Camille Robert', start: '01/02/2026', end: '—', status: 'En cours', tone: 'success' as const },
    { tenant: 'Élodie Marchal', start: '01/05/2024', end: '31/12/2025', status: 'Terminé', tone: 'neutral' as const },
  ];

  protected readonly paymentHistory = [
    { month: 'Avril 2026', expected: '620,00 €', paid: '0,00 €', remaining: '620,00 €', status: 'En retard', tone: 'error' as const },
    { month: 'Mars 2026', expected: '620,00 €', paid: '620,00 €', remaining: '0,00 €', status: 'Payé', tone: 'success' as const },
    { month: 'Février 2026', expected: '620,00 €', paid: '620,00 €', remaining: '0,00 €', status: 'Payé', tone: 'success' as const },
  ];
}
