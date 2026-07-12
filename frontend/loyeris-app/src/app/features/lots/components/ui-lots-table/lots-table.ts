import { Component, computed, inject, input, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

export type LotListItem = {
  readonly id: string;
  readonly reference: string;
  readonly sci: string;
  readonly address: string;
  readonly type: string;
  readonly occupation: string;
  readonly amount: string;
  readonly amountBreakdown: string;
  readonly archived: boolean;
  readonly hasReceivableAmount: boolean;
  readonly status: string;
  readonly statusTone: 'success' | 'warning' | 'neutral';
  readonly editUrl: string;
};

@Component({
  selector: 'ui-lots-table',
  imports: [RouterLink, StatusBadge],
  templateUrl: './lots-table.html',
  styleUrl: './lots-table.css',
})
export class LotsTable {
  private readonly router = inject(Router);
  readonly lots = input.required<readonly LotListItem[]>();
  protected readonly showAllArchived = signal(false);
  protected readonly visibleLots = computed(() =>
    this.showAllArchived()
      ? this.lots()
      : this.lots().filter((lot) => !lot.archived || lot.hasReceivableAmount),
  );

  protected archivedVisibilityChanged(event: Event): void {
    this.showAllArchived.set((event.target as HTMLInputElement).checked);
  }

  protected openLot(url: string): void {
    void this.router.navigateByUrl(url);
  }
}
