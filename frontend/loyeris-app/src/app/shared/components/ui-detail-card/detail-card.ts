import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-detail-card',
  templateUrl: './detail-card.html',
  styleUrl: './detail-card.css',
})
export class DetailCard {
  readonly title = input.required<string>();
  readonly description = input.required<string>();
}
