import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-dashboard-card',
  templateUrl: './dashboard-card.html',
  styleUrl: './dashboard-card.css',
})
export class DashboardCard {
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly linkLabel = input<string>();
  readonly linkUrl = input<string>('#');
}
