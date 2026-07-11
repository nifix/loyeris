import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import {
  AbstractControl,
  FormBuilder,
  ReactiveFormsModule,
  ValidationErrors,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, take } from 'rxjs';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FormField } from '../../components/ui-form-field/form-field';
import { PasswordChecklist } from '../../components/ui-password-checklist/password-checklist';
import { IdentityAccessApi } from '../../services/identity-access-api';

interface RegistrationErrorViewModel {
  message: string;
  showLoginAction: boolean;
  title: string;
}

export type RegistrationSubmissionErrorCode = 'email-exists' | 'generic';

const trimmedRequired: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>
  typeof control.value === 'string' && control.value.trim().length > 0 ? null : { required: true };

// Password confirmation is a browser-only safeguard and is deliberately omitted from the API payload.
const matchingPasswords: ValidatorFn = (control: AbstractControl): ValidationErrors | null =>
  control.get('password')?.value === control.get('passwordConfirmation')?.value
    ? null
    : { passwordMismatch: true };

@Component({
  selector: 'app-register-page',
  imports: [
    AuthLayout,
    AuthSubmissionError,
    BrandMark,
    FormField,
    PasswordChecklist,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './register-page.html',
  styleUrl: './register-page.css',
})
export class RegisterPage {
  readonly submissionErrorCode = input<RegistrationSubmissionErrorCode | null>(null);
  readonly submitting = input(false);

  private readonly api = inject(IdentityAccessApi);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly queryParams = toSignal(inject(ActivatedRoute).queryParamMap, {
    requireSync: true,
  });
  private readonly apiErrorCode = signal<RegistrationSubmissionErrorCode | null>(null);
  private readonly localSubmitting = signal(false);
  private readonly submitted = signal(false);

  protected readonly form = this.formBuilder.nonNullable.group(
    {
      firstName: ['', [trimmedRequired, Validators.maxLength(120)]],
      lastName: ['', [trimmedRequired, Validators.maxLength(120)]],
      email: ['', [trimmedRequired, Validators.email, Validators.maxLength(320)]],
      password: [
        '',
        [
          Validators.required,
          Validators.minLength(8),
          Validators.maxLength(128),
          // Unicode-aware expressions mirror the backend's char-based password policy.
          Validators.pattern(/\p{Lu}/u),
          Validators.pattern(/\p{Nd}/u),
          Validators.pattern(/[^\p{L}\p{N}]/u),
        ],
      ],
      passwordConfirmation: ['', Validators.required],
      termsAccepted: [false, Validators.requiredTrue],
    },
    { validators: matchingPasswords },
  );

  protected readonly isSubmitting = computed(() => this.submitting() || this.localSubmitting());

  protected readonly submissionError = computed<RegistrationErrorViewModel | null>(() => {
    switch (
      this.apiErrorCode() ?? this.submissionErrorCode() ?? this.queryParams().get('error')
    ) {
      case 'email-exists':
        return {
          title: 'Cette adresse email est déjà utilisée',
          message: 'Un compte Loyeris est déjà associé à cette adresse.',
          showLoginAction: true,
        };
      case 'generic':
        return {
          title: 'Création du compte impossible',
          message: 'Une erreur inattendue est survenue. Vérifiez votre connexion puis réessayez.',
          showLoginAction: false,
        };
      default:
        return null;
    }
  });

  protected fieldError(
    fieldName: 'firstName' | 'lastName' | 'email' | 'password',
  ): string | undefined {
    const control = this.form.controls[fieldName];

    if (!control.invalid || (!control.touched && !this.submitted())) {
      return undefined;
    }

    if (control.hasError('required')) {
      return 'Ce champ est obligatoire.';
    }

    if (fieldName === 'email') {
      return 'Saisissez une adresse email valide.';
    }

    if (fieldName === 'password') {
      return 'Le mot de passe ne respecte pas les exigences indiquées.';
    }

    return 'Ce champ est trop long.';
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
      ? 'Confirmez votre mot de passe.'
      : 'Les deux mots de passe doivent être identiques.';
  }

  protected submit(): void {
    // Prevent duplicate account creation while the first HTTP request is still pending.
    if (this.isSubmitting()) {
      return;
    }

    this.submitted.set(true);
    this.apiErrorCode.set(null);
    this.form.markAllAsTouched();

    if (this.form.invalid) {
      return;
    }

    this.localSubmitting.set(true);
    const value = this.form.getRawValue();

    // Trim identity fields at the boundary while preserving the password exactly as entered.
    this.api
      .registerAccount({
        firstName: value.firstName.trim(),
        lastName: value.lastName.trim(),
        email: value.email.trim(),
        password: value.password,
        termsAccepted: value.termsAccepted,
      })
      .pipe(
        take(1),
        finalize(() => this.localSubmitting.set(false)),
      )
      .subscribe({
        next: () =>
          void this.router.navigate(['/verify-email'], { queryParams: { status: 'pending' } }),
        error: (error: HttpErrorResponse) =>
          this.apiErrorCode.set(error.status === 409 ? 'email-exists' : 'generic'),
      });
  }
}
