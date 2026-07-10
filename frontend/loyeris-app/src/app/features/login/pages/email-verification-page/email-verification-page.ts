import { Component, computed, inject, input } from '@angular/core';
import { toSignal } from '@angular/core/rxjs-interop';
import { ActivatedRoute, RouterLink } from '@angular/router';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import {
  EmailVerificationMessage,
  type VerificationStatus,
} from '../../components/ui-email-verification-message/email-verification-message';

@Component({
  selector: 'app-email-verification-page',
  imports: [AuthLayout, BrandMark, EmailVerificationMessage, RouterLink],
  templateUrl: './email-verification-page.html',
  styleUrl: './email-verification-page.css',
})
export class EmailVerificationPage {
  readonly verificationStatus = input<VerificationStatus | null>(null);

  private readonly queryParams = toSignal(inject(ActivatedRoute).queryParamMap, {
    requireSync: true,
  });

  protected readonly status = computed<VerificationStatus>(() =>
    this.verificationStatus() ??
    (this.queryParams().get('status') === 'error' ? 'error' : 'success'),
  );
}
