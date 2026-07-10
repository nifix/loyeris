import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-auth-layout',
  templateUrl: './auth-layout.html',
  styleUrl: './auth-layout.css',
})
export class AuthLayout {
  readonly centered = input(false);
  readonly widePanel = input(false);
}
