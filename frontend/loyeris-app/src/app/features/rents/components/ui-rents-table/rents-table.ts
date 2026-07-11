import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

export type RentListItem = {
  readonly tenant: string;
  readonly lot: string;
  readonly sci: string;
  readonly due: string;
  readonly paid: string;
  readonly remaining: string;
  readonly paymentDate: string;
  readonly status: string;
  readonly statusTone: 'success' | 'warning' | 'error';
  readonly action: string;
  readonly actionTone: 'primary' | 'warning' | 'ghost';
  readonly actionUrl: string;
};

@Component({
  selector: 'ui-rents-table',
  imports: [RouterLink, StatusBadge],
  templateUrl: './rents-table.html',
  styleUrl: './rents-table.css',
})
export class RentsTable {
  readonly rents = input.required<readonly RentListItem[]>();
  readonly period = input('Avril 2026');
}
