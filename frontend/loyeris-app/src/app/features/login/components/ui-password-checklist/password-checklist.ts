import { Component, computed, input } from '@angular/core';

interface PasswordRequirement {
  label: string;
  met: boolean;
}

@Component({
  selector: 'ui-password-checklist',
  templateUrl: './password-checklist.html',
  styleUrl: './password-checklist.css',
})
export class PasswordChecklist {
  readonly password = input('');

  protected readonly requirements = computed<PasswordRequirement[]>(() => {
    const password = this.password();

    // Unicode categories keep accented uppercase letters and non-ASCII digits consistent with .NET.
    return [
      {
        label: '8 caractères minimum',
        met: password.length >= 8,
      },
      {
        label: 'Une majuscule et un chiffre',
        met: /\p{Lu}/u.test(password) && /\p{Nd}/u.test(password),
      },
      {
        label: 'Un caractère spécial',
        met: /[^\p{L}\p{N}]/u.test(password),
      },
    ];
  });
}
