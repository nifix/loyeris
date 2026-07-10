import { moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { BrandMark } from '../../../../shared/components/ui-brand-mark/brand-mark';
import { AuthLayout } from './auth-layout';

const meta: Meta<AuthLayout> = {
  title: 'Features/Login/Components/Auth Layout',
  component: AuthLayout,
  tags: ['autodocs'],
  parameters: {
    layout: 'fullscreen',
  },
  decorators: [
    moduleMetadata({
      imports: [AuthLayout, BrandMark],
    }),
  ],
};

export default meta;

type Story = StoryObj<AuthLayout>;

export const Default: Story = {
  render: () => ({
    template: `
      <ui-auth-layout>
        <div auth-intro class="space-y-6">
          <div class="flex items-center gap-4">
            <ui-brand-mark />
            <p class="text-2xl font-semibold">Loyeris</p>
          </div>
          <h1 class="max-w-2xl text-5xl font-black leading-tight">
            Une gestion locative enfin claire.
          </h1>
        </div>

        <div auth-panel class="space-y-6">
          <div class="badge badge-primary badge-outline">Espace gestionnaire</div>
          <h2 class="text-3xl font-bold">Bienvenue</h2>
          <button class="btn btn-primary btn-block">Continuer</button>
        </div>
      </ui-auth-layout>
    `,
  }),
};

export const CenteredPanel: Story = {
  render: () => ({
    template: `
      <ui-auth-layout [centered]="true">
        <div auth-panel class="space-y-6 text-center">
          <ui-brand-mark size="sm" />
          <h2 class="text-3xl font-bold">Adresse vérifiée</h2>
          <p class="text-base-content/70">Votre compte est prêt.</p>
          <button class="btn btn-primary btn-block">Continuer</button>
        </div>
      </ui-auth-layout>
    `,
  }),
};
