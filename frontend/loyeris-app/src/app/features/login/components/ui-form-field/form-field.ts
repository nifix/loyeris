import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-form-field',
  templateUrl: './form-field.html',
  styleUrl: './form-field.css',
})
export class FormField {
  readonly autocomplete = input<string>('');
  readonly fieldId = input.required<string>();
  readonly forgotLabel = input<string>();
  readonly label = input.required<string>();
  readonly placeholder = input.required<string>();
  readonly required = input(false);
  readonly type = input<'email' | 'password' | 'text'>('text');
}
