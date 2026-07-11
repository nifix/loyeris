import { Component, forwardRef, input, signal } from '@angular/core';
import { ControlValueAccessor, NG_VALUE_ACCESSOR } from '@angular/forms';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'ui-form-field',
  imports: [RouterLink],
  templateUrl: './form-field.html',
  styleUrl: './form-field.css',
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: forwardRef(() => FormField),
      multi: true,
    },
  ],
})
export class FormField implements ControlValueAccessor {
  readonly autocomplete = input<string>('');
  readonly errorMessage = input<string>();
  readonly fieldId = input.required<string>();
  readonly forgotLabel = input<string>();
  readonly forgotUrl = input('#');
  readonly label = input.required<string>();
  readonly placeholder = input.required<string>();
  readonly required = input(false);
  readonly type = input<'email' | 'password' | 'text'>('text');

  protected readonly disabled = signal(false);
  protected readonly value = signal('');

  // These callbacks bridge the encapsulated native input with Angular reactive forms.
  private onChange: (value: string) => void = () => undefined;
  private onTouched: () => void = () => undefined;

  writeValue(value: string | null): void {
    this.value.set(value ?? '');
  }

  registerOnChange(onChange: (value: string) => void): void {
    this.onChange = onChange;
  }

  registerOnTouched(onTouched: () => void): void {
    this.onTouched = onTouched;
  }

  setDisabledState(isDisabled: boolean): void {
    this.disabled.set(isDisabled);
  }

  protected handleInput(event: Event): void {
    const value = (event.target as HTMLInputElement).value;
    this.value.set(value);
    this.onChange(value);
  }

  protected handleBlur(): void {
    this.onTouched();
  }
}
