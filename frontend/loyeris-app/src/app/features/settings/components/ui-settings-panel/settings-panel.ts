import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-settings-panel',
  templateUrl: './settings-panel.html',
  styleUrl: './settings-panel.css',
})
export class SettingsPanel {
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly avatarLabel = input<string>();
}
