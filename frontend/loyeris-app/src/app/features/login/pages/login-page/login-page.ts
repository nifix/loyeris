import { Component, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FeatureCard } from '../../components/ui-feature-card/feature-card';
import { FormField } from '../../components/ui-form-field/form-field';

interface LoginErrorViewModel {
  message: string;
  title: string;
}

export type LoginSubmissionErrorCode =
  | 'account-not-found'
  | 'email-not-verified'
  | 'generic'
  | 'invalid-credentials';

@Component({
  selector: 'app-login-page',
  imports: [AuthLayout, AuthSubmissionError, FeatureCard, FormField, BrandMark, RouterLink],
  templateUrl: './login-page.html',
  styleUrl: './login-page.css',
})
export class LoginPage {
  readonly submissionErrorCode = input<LoginSubmissionErrorCode | null>(null);

  private readonly queryParams = toSignal(inject(ActivatedRoute).queryParamMap, {
    requireSync: true,
  });

  protected readonly submissionError = computed<LoginErrorViewModel | null>(() => {
    switch (this.submissionErrorCode() ?? this.queryParams().get('error')) {
      case 'account-not-found':
      case 'invalid-credentials':
        return {
          title: 'Identifiants incorrects',
          message: 'L’adresse email ou le mot de passe renseigné est incorrect.',
        };
      case 'email-not-verified':
        return {
          title: 'Adresse email non vérifiée',
          message: 'Validez votre adresse email avant de vous connecter à votre espace.',
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
}
