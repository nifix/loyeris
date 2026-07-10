import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-auth-submission-error',
  templateUrl: './auth-submission-error.html',
  styleUrl: './auth-submission-error.css',
})
export class AuthSubmissionError {
  readonly message = input.required<string>();
  readonly title = input.required<string>();
}
