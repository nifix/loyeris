import { Component } from '@angular/core';
import { AppShell } from '../../../../core/layouts/app-shell/app-shell';
import { PageHeader } from '../../../../shared/components/ui-page-header/page-header';
import { SettingsPanel } from '../../components/ui-settings-panel/settings-panel';

@Component({
  selector: 'app-settings-page',
  imports: [AppShell, PageHeader, SettingsPanel],
  templateUrl: './settings-page.html',
  styleUrl: './settings-page.css',
})
export class SettingsPage {
  protected readonly headerChips = ['Compte actif', "Dernière connexion : aujourd'hui"];
}
