import { Component, input } from '@angular/core';

@Component({
  selector: 'app-auth-form-field',
  templateUrl: './auth-form-field.html',
  styleUrl: './auth-form-field.css',
})

export class AuthFormField {
  readonly autocomplete = input<string>('');
  readonly fieldId = input.required<string>();
  readonly forgotLabel = input<string>();
  readonly label = input.required<string>();
  readonly placeholder = input.required<string>();
  readonly type = input<'email' | 'password' | 'text'>('text');
}
