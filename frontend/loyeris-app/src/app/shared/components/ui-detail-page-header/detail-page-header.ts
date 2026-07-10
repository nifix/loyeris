import { Component, input } from '@angular/core';
import { StatusBadge } from '../ui-status-badge/status-badge';

@Component({
  selector: 'ui-detail-page-header',
  imports: [StatusBadge],
  templateUrl: './detail-page-header.html',
  styleUrl: './detail-page-header.css',
})
export class DetailPageHeader {
  readonly badge = input.required<string>();
  readonly status = input.required<string>();
  readonly statusTone = input<'success' | 'warning' | 'error' | 'neutral'>('neutral');
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly chips = input<readonly string[]>([]);
  readonly secondaryActionLabel = input.required<string>();
  readonly primaryActionLabel = input.required<string>();
  readonly primaryActionUrl = input('#');
}
