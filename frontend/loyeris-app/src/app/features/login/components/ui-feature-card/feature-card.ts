import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-feature-card',
  templateUrl: './feature-card.html',
  styleUrl: './feature-card.css',
})
export class FeatureCard {
  readonly header = input.required<string>();
  readonly title = input.required<string>();
  readonly description = input.required<string>();
}
