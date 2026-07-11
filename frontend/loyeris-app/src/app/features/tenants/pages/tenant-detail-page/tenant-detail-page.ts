import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { DetailCard } from '../../../../shared/components/ui-detail-card/detail-card';
import { DetailPageHeader } from '../../../../shared/components/ui-detail-page-header/detail-page-header';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

@Component({
  selector: 'app-tenant-detail-page',
  imports: [AppShell, DetailCard, DetailPageHeader, RouterLink, StatusBadge],
  templateUrl: './tenant-detail-page.html',
  styleUrl: './tenant-detail-page.css',
})
export class TenantDetailPage {
  protected readonly headerChips = [
    'Entrée le 1 février 2026',
    '620 € charges comprises',
    '1 dossier de retard en cours',
  ];

  protected readonly rentalHistory = [
    {
      lot: 'Lot A02',
      sci: 'SCI Les Tilleuls',
      start: '01/02/2026',
      end: '—',
      status: 'En cours',
    },
  ];

  protected readonly paymentHistory = [
    { month: 'Avril 2026', due: '620 €', paid: '0 €', remaining: '620 €', status: 'En retard', tone: 'error' as const },
    { month: 'Mars 2026', due: '620 €', paid: '620 €', remaining: '0 €', status: 'Payé', tone: 'success' as const },
    { month: 'Février 2026', due: '620 €', paid: '620 €', remaining: '0 €', status: 'Payé', tone: 'success' as const },
  ];
}
