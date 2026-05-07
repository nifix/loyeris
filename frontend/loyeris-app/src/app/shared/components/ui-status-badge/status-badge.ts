import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-status-badge',
  templateUrl: './status-badge.html',
  styleUrl: './status-badge.css',
})
export class StatusBadge {
  readonly tone = input<'success' | 'warning' | 'error' | 'neutral'>('neutral');
}
