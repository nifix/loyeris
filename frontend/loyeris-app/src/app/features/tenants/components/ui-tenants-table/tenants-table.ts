import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

export type TenantListItem = {
  readonly name: string;
  readonly sci: string;
  readonly currentLot: string;
  readonly entryDate: string;
  readonly email: string;
  readonly phone: string;
  readonly status: string;
  readonly statusTone: 'success' | 'neutral';
  readonly detailUrl: string;
};

@Component({
  selector: 'ui-tenants-table',
  imports: [RouterLink, StatusBadge],
  templateUrl: './tenants-table.html',
  styleUrl: './tenants-table.css',
})
export class TenantsTable {
  readonly tenants = input.required<readonly TenantListItem[]>();
}
