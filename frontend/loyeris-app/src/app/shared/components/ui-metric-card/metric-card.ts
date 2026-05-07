import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-metric-card',
  templateUrl: './metric-card.html',
  styleUrl: './metric-card.css',
})
export class MetricCard {
  readonly title = input.required<string>();
  readonly value = input.required<string>();
  readonly description = input.required<string>();
  readonly tone = input<'primary' | 'success' | 'warning' | 'neutral'>('neutral');
}
