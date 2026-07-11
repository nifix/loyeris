import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'ui-dashboard-card',
  imports: [RouterLink],
  templateUrl: './dashboard-card.html',
  styleUrl: './dashboard-card.css',
})
export class DashboardCard {
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly linkLabel = input<string>();
  readonly linkUrl = input<string>('#');
}
