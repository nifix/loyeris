import { Component, computed, inject, input, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { finalize, take } from 'rxjs';

import { IdentityAccessApi } from '../../../../core/auth/identity-access-api';
import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FormField } from '../../components/ui-form-field/form-field';

export type ForgotPasswordState = 'form' | 'submitting' | 'success' | 'error';

@Component({
  selector: 'app-forgot-password-page',
  imports: [
    AuthLayout,
    AuthSubmissionError,
    BrandMark,
    FormField,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './forgot-password-page.html',
  styleUrl: './forgot-password-page.css',
})
export class ForgotPasswordPage {
  readonly presentationState = input<ForgotPasswordState | null>(null);

  private readonly api = inject(IdentityAccessApi);
  private readonly formBuilder = inject(FormBuilder);
  private readonly localState = signal<ForgotPasswordState>('form');
  private readonly submitted = signal(false);

  protected readonly state = computed(() => this.presentationState() ?? this.localState());
  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(320)]],
  });

  protected emailError(): string | undefined {
    const control = this.form.controls.email;
    return control.invalid && (control.touched || this.submitted())
      ? 'Saisissez une adresse email valide.'
      : undefined;
  }

  protected submit(): void {
    if (this.state() === 'submitting') {
      return;
    }

    this.submitted.set(true);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    this.localState.set('submitting');
    this.api
      .requestPasswordReset(this.form.controls.email.value.trim())
      .pipe(
        take(1),
        finalize(() => {
          if (this.localState() === 'submitting') {
            this.localState.set('form');
          }
        }),
      )
      .subscribe({
        next: () => this.localState.set('success'),
        error: () => this.localState.set('error'),
      });
  }
}
