import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { SettingsPanel } from './settings-panel';

const meta: Meta<SettingsPanel> = {
  title: 'Features/Settings/Components/Settings Panel',
  component: SettingsPanel,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({ imports: [SettingsPanel] }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[40rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: {
    title: 'Sécurité',
    description: "Protégez l'accès à votre compte.",
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-settings-panel [title]="title" [description]="description" [avatarLabel]="avatarLabel">
        <div class="rounded-lg border border-base-200 bg-slate-50 p-4">
          <p class="font-semibold text-slate-900">Mot de passe</p>
          <p class="text-sm text-slate-500">Dernière modification il y a 3 mois</p>
        </div>
      </ui-settings-panel>
    `,
  }),
};

export default meta;

type Story = StoryObj<SettingsPanel>;

export const Security: Story = {};

export const Profile: Story = {
  args: {
    title: 'Profil',
    description: "Vos informations personnelles visibles dans l'application.",
    avatarLabel: 'NF',
  },
};
