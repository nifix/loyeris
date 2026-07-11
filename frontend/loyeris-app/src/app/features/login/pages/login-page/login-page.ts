import { HttpErrorResponse } from '@angular/common/http';
import { Component, computed, inject, input, signal } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, take } from 'rxjs';

import { AuthSession } from '../../../../core/auth/auth-session';
import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FeatureCard } from '../../components/ui-feature-card/feature-card';
import { FormField } from '../../components/ui-form-field/form-field';

interface LoginErrorViewModel {
  message: string;
  title: string;
}

interface ProblemDetails {
  errorCode?: string;
}

export type LoginSubmissionErrorCode =
  | 'account-not-found'
  | 'email-not-verified'
  | 'generic'
  | 'invalid-credentials';

@Component({
  selector: 'app-login-page',
  imports: [
    AuthLayout,
    AuthSubmissionError,
    FeatureCard,
    FormField,
    BrandMark,
    ReactiveFormsModule,
    RouterLink,
  ],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  readonly submissionErrorCode = input<LoginSubmissionErrorCode | null>(null);
  readonly submitting = input(false);

  private readonly authSession = inject(AuthSession);
  private readonly formBuilder = inject(FormBuilder);
  private readonly router = inject(Router);
  private readonly queryParams = toSignal(inject(ActivatedRoute).queryParamMap, {
    requireSync: true,
  });
  private readonly apiErrorCode = signal<LoginSubmissionErrorCode | null>(null);
  private readonly localSubmitting = signal(false);
  private readonly submitted = signal(false);

  protected readonly form = this.formBuilder.nonNullable.group({
    email: ['', [Validators.required, Validators.email, Validators.maxLength(320)]],
    password: ['', Validators.required],
    rememberMe: [true],
  });
  protected readonly isSubmitting = computed(() => this.submitting() || this.localSubmitting());

  protected readonly submissionError = computed<LoginErrorViewModel | null>(() => {
    switch (
      this.apiErrorCode() ?? this.submissionErrorCode() ?? this.queryParams().get('error')
    ) {
      case 'account-not-found':
      case 'invalid-credentials':
        return {
          title: 'Identifiants incorrects',
          message: 'L’adresse email ou le mot de passe renseigné est incorrect.',
        };
      case 'email-not-verified':
        return {
          title: 'Adresse email non vérifiée',
          message:
            'Un nouveau lien de vérification vient de vous être envoyé. Validez votre adresse avant de vous connecter.',
        };
      case 'generic':
        return {
          title: 'Connexion impossible',
          message: 'Une erreur inattendue est survenue. Vérifiez votre connexion puis réessayez.',
        };
      default:
        return null;
    }
  });

  protected fieldError(fieldName: 'email' | 'password'): string | undefined {
    const control = this.form.controls[fieldName];
    if (!control.invalid || (!control.touched && !this.submitted())) {
      return undefined;
    }

    return fieldName === 'email'
      ? 'Saisissez une adresse email valide.'
      : 'Saisissez votre mot de passe.';
  }

  protected submit(): void {
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
    this.authSession
      .login({
        email: value.email.trim(),
        password: value.password,
        rememberMe: value.rememberMe,
      })
      .pipe(
        take(1),
        finalize(() => this.localSubmitting.set(false)),
      )
      .subscribe({
        next: () => void this.router.navigate(['/dashboard']),
        error: (error: HttpErrorResponse) => {
          const errorCode = (error.error as ProblemDetails | null)?.errorCode;
          this.apiErrorCode.set(
            errorCode === 'identity.email_not_verified'
              ? 'email-not-verified'
              : error.status === 401
                ? 'invalid-credentials'
                : 'generic',
          );
        },
      });
  }
}
