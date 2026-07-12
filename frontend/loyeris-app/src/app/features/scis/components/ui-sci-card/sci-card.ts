import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';
import { StatusBadge } from '../../../../shared/components/ui-status-badge/status-badge';

@Component({
  selector: 'ui-sci-card',
  imports: [RouterLink, StatusBadge],
  templateUrl: './sci-card.html',
  styleUrl: './sci-card.css',
})
export class SciCard {
  readonly id = input.required<string>();
  readonly name = input.required<string>();
  readonly status = input.required<'Active' | 'Archived'>();
  readonly address = input.required<string>();
  readonly createdAt = input.required<string>();
  readonly incorporatedOn = input<string>();
  readonly siren = input<string>();
  readonly taxRegime = input.required<string>();
}
