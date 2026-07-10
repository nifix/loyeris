import { Component } from '@angular/core';

@Component({
  selector: 'ui-password-checklist',
  templateUrl: './password-checklist.html',
  styleUrl: './password-checklist.css',
})
export class PasswordChecklist {
  protected readonly requirements = [
    '8 caractères minimum',
    'Une majuscule et un chiffre',
    'Un caractère spécial',
  ];
}
