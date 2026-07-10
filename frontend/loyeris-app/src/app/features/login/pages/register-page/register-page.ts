import { Component, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import { AuthSubmissionError } from '../../components/ui-auth-submission-error/auth-submission-error';
import { FormField } from '../../components/ui-form-field/form-field';
import { PasswordChecklist } from '../../components/ui-password-checklist/password-checklist';

interface RegistrationErrorViewModel {
  message: string;
  showLoginAction: boolean;
  title: string;
}

export type RegistrationSubmissionErrorCode = 'email-exists' | 'generic';

@Component({
  selector: 'app-register-page',
  imports: [AuthLayout, AuthSubmissionError, BrandMark, FormField, PasswordChecklist, RouterLink],
  templateUrl: './register-page.html',
  styleUrl: './register-page.css',
})
export class RegisterPage {
  readonly submissionErrorCode = input<RegistrationSubmissionErrorCode | null>(null);

  private readonly queryParams = toSignal(inject(ActivatedRoute).queryParamMap, {
    requireSync: true,
  });

  protected readonly submissionError = computed<RegistrationErrorViewModel | null>(() => {
    switch (this.submissionErrorCode() ?? this.queryParams().get('error')) {
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
}
