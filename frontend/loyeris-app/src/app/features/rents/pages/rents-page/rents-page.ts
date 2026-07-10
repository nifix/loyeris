import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { MetricCard } from '../../../../shared/components/ui-metric-card/metric-card';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { RentsTable, type RentListItem } from '../../components/ui-rents-table/rents-table';

@Component({
  selector: 'app-rents-page',
  imports: [AppShell, MetricCard, PageHeader, RentsTable],
  templateUrl: './rents-page.html',
  styleUrl: './rents-page.css',
})
export class RentsPage {
  protected readonly headerChips = [
    '4 320 € attendus',
    '780 € restants',
    '2 dossiers à traiter',
  ];

  protected readonly metrics = [
    { title: 'Montant attendu', value: '4 320 €', description: 'Loyers générés pour avril 2026', tone: 'primary' as const },
    { title: 'Montant encaissé', value: '3 540 €', description: 'Paiements enregistrés', tone: 'success' as const },
    { title: 'Reste à recevoir', value: '780 €', description: 'Montant non soldé', tone: 'warning' as const },
    { title: 'Retards', value: '2', description: '1 complet · 1 partiel', tone: 'error' as const },
  ];

  protected readonly rents: readonly RentListItem[] = [
    { tenant: 'Sophie Martin', lot: 'Lot A01', sci: 'SCI Les Tilleuls', due: '680 €', paid: '680 €', remaining: '0 €', paymentDate: '26/04/2026', status: 'Payé', statusTone: 'success', action: 'Voir →', actionTone: 'ghost', actionUrl: '/tenants/sophie-martin' },
    { tenant: 'Camille Robert', lot: 'Lot A02', sci: 'SCI Les Tilleuls', due: '620 €', paid: '0 €', remaining: '620 €', paymentDate: '—', status: 'En retard', statusTone: 'error', action: 'Encaisser', actionTone: 'primary', actionUrl: '#' },
    { tenant: 'Louis Mercier', lot: 'Lot B01', sci: 'SCI Carnot', due: '540 €', paid: '380 €', remaining: '160 €', paymentDate: '21/04/2026', status: 'Partiel', statusTone: 'warning', action: 'Compléter', actionTone: 'warning', actionUrl: '#' },
    { tenant: 'Mathieu Leroy', lot: 'Lot B03', sci: 'SCI Carnot', due: '720 €', paid: '720 €', remaining: '0 €', paymentDate: '24/04/2026', status: 'Payé', statusTone: 'success', action: 'Voir →', actionTone: 'ghost', actionUrl: '/tenants/mathieu-leroy' },
    { tenant: 'Julie Lambert', lot: 'Lot C01', sci: 'SCI Les Tilleuls', due: '950 €', paid: '950 €', remaining: '0 €', paymentDate: '23/04/2026', status: 'Payé', statusTone: 'success', action: 'Voir →', actionTone: 'ghost', actionUrl: '/tenants/julie-lambert' },
  ];
}
