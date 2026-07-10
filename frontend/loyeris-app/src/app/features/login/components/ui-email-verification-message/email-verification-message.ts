import { Component, input } from '@angular/core';

export type VerificationStatus = 'success' | 'error';

@Component({
  selector: 'ui-email-verification-message',
  templateUrl: './email-verification-message.html',
  styleUrl: './email-verification-message.css',
})
export class EmailVerificationMessage {
  readonly status = input.required<VerificationStatus>();
}
