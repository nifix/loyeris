import { Component, input } from '@angular/core';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

@Component({
  selector: 'ui-sci-card',
  imports: [StatusBadge],
  templateUrl: './sci-card.html',
  styleUrl: './sci-card.css',
})
export class SciCard {
  readonly name = input.required<string>();
  readonly status = input('Active');
  readonly address = input.required<string>();
  readonly createdAt = input.required<string>();
  readonly lots = input.required<number>();
  readonly tenants = input.required<number>();
  readonly monthlyRent = input.required<string>();
  readonly occupancy = input.required<string>();
  readonly performance = input.required<number>();
  readonly performanceTone = input<'primary' | 'success'>('primary');
  readonly lotsUrl = input('#');
  readonly rentsUrl = input('#');
}
