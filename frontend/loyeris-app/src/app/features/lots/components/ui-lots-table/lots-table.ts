import { Component, input } from '@angular/core';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

export type LotListItem = {
  readonly reference: string;
  readonly sci: string;
  readonly address: string;
  readonly type: string;
  readonly occupation: string;
  readonly rent: string;
  readonly status: string;
  readonly statusTone: 'success' | 'warning';
  readonly detailUrl: string;
};

@Component({
  selector: 'ui-lots-table',
  imports: [StatusBadge],
  templateUrl: './lots-table.html',
  styleUrl: './lots-table.css',
})
export class LotsTable {
  readonly lots = input.required<readonly LotListItem[]>();
}
