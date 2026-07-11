import { Component, inject, input, OnInit, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { take } from 'rxjs';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from '../../components/ui-auth-layout/auth-layout';
import {
  EmailVerificationMessage,
  type VerificationStatus,
} from '../../components/ui-email-verification-message/email-verification-message';
import { IdentityAccessApi } from '../../../../core/auth/identity-access-api';

@Component({
  selector: 'app-email-verification-page',
  imports: [AuthLayout, BrandMark, EmailVerificationMessage, RouterLink],
  templateUrl: './email-verification-page.html',
  styleUrl: './email-verification-page.css',
})
export class EmailVerificationPage implements OnInit {
  readonly verificationStatus = input<VerificationStatus | null>(null);

  private readonly api = inject(IdentityAccessApi);
  private readonly route = inject(ActivatedRoute);

  protected readonly status = signal<VerificationStatus>('pending');

  ngOnInit(): void {
    // Storybook can force a visual state without triggering a real verification request.
    if (this.verificationStatus()) {
      this.status.set(this.verificationStatus()!);
      return;
    }

    const token = this.route.snapshot.queryParamMap.get('token');

    if (!token) {
      this.status.set(
        this.route.snapshot.queryParamMap.get('status') === 'pending' ? 'pending' : 'error',
      );
      return;
    }

    // Opening the email link only renders the page; the state change is performed explicitly via POST.
    this.status.set('verifying');
    this.api
      .verifyEmail(token)
      .pipe(take(1))
      .subscribe({
        next: () => this.status.set('success'),
        error: () => this.status.set('error'),
      });
  }
}
