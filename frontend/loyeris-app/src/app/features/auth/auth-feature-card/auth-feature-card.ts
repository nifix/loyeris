import { Component, input } from '@angular/core';

@Component({
  selector: 'app-auth-feature-card',
  templateUrl: './auth-feature-card.html',
  styleUrl: './auth-feature-card.css',
})

export class AuthFeatureCard {
  readonly header = input.required<string>();
  readonly title = input.required<string>();
  readonly description = input.required<string>();
}
