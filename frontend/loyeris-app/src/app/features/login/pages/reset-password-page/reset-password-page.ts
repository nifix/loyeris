import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, OnInit, signal } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { take } from 'rxjs';

import { IdentityAccessApi } from '../../../../core/auth/identity-access-api';
import { AuthSession } from '../../../../core/auth/auth-session';
import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FormField } from '../../components/ui-form-field/form-field';
import { PasswordChecklist } from '../../components/ui-password-checklist/password-checklist';

export type ResetPasswordState =
  | 'validating'
  | 'form'
  | 'submitting'
  | 'success'
  | 'invalid'
  | 'validation-error'
  | 'submission-error';

const matchingPasswords: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>
  control.get('password')?.value === control.get('passwordConfirmation')?.value
    ? null
    : { passwordMismatch: true };

@Component({
  selector: 'app-reset-password-page',
  imports: [
    AuthLayout,
    AuthSubmissionError,
    BrandMark,
    FormField,
    PasswordChecklist,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './reset-password-page.html',
  styleUrl: './reset-password-page.css',
})
export class ResetPasswordPage implements OnInit {
  readonly presentationState = input<ResetPasswordState | null>(null);

  private readonly api = inject(IdentityAccessApi);
  private readonly authSession = inject(AuthSession);
  private readonly formBuilder = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly localState = signal<ResetPasswordState>('validating');
  private readonly submitted = signal(false);
  private token: string | null = null;

  protected readonly state = computed(() => this.presentationState() ?? this.localState());
  protected readonly form = this.formBuilder.nonNullable.group(
    {
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(128),
          Validators.pattern(/\p{Lu}/u),
          Validators.pattern(/\p{Nd}/u),
          Validators.pattern(/[^\p{L}\p{N}]/u),
        ],
      ],
      passwordConfirmation: ['', Validators.required],
    },
    { validators: matchingPasswords },
  );

  ngOnInit(): void {
    // Storybook supplies a visual state and must not trigger a real token validation request.
    if (this.presentationState()) {
      return;
    }

    this.token = this.route.snapshot.queryParamMap.get('token');
    if (!this.token) {
      this.localState.set('invalid');
      return;
    }

    this.validateToken();
  }

  protected passwordError(): string | undefined {
    const control = this.form.controls.password;
    if (!control.invalid || (!control.touched && !this.submitted())) {
      return undefined;
    }

    return control.hasError('required')
      ? 'Saisissez un nouveau mot de passe.'
      : 'Le mot de passe ne respecte pas les exigences indiquées.';
  }

  protected confirmationError(): string | undefined {
    const control = this.form.controls.passwordConfirmation;
    if (
      (!control.touched && !this.submitted()) ||
      (!control.invalid && !this.form.hasError('passwordMismatch'))
    ) {
      return undefined;
    }

    return control.hasError('required')
      ? 'Confirmez votre nouveau mot de passe.'
      : 'Les deux mots de passe doivent être identiques.';
  }

  protected validateToken(): void {
    if (!this.token) {
      this.localState.set('invalid');
      return;
    }

    this.localState.set('validating');
    this.api
      .validatePasswordResetToken(this.token)
      .pipe(take(1))
      .subscribe({
        next: () => this.localState.set('form'),
        error: (error: HttpErrorResponse) =>
          this.localState.set(error.status === 422 ? 'invalid' : 'validation-error'),
      });
  }

  protected submit(): void {
    if (this.state() === 'submitting' || !this.token) {
      return;
    }

    this.submitted.set(true);
    this.form.markAllAsTouched();
    if (this.form.invalid) {
      return;
    }

    this.localState.set('submitting');
    this.api
      .resetPassword({ token: this.token, newPassword: this.form.controls.password.value })
      .pipe(take(1))
      .subscribe({
        next: () => {
          // A password reset revokes every server session, so the in-memory access token must also disappear.
          this.authSession.clear();
          this.localState.set('success');
        },
        error: (error: HttpErrorResponse) =>
          this.localState.set(error.status === 422 ? 'invalid' : 'submission-error'),
      });
  }
}
