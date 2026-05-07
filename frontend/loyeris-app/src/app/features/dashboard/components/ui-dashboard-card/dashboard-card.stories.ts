import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { DashboardCard } from './dashboard-card';

const meta: Meta<DashboardCard> = {
  title: 'Features/Dashboard/Components/Dashboard Card',
  component: DashboardCard,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [DashboardCard],
    }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="max-w-2xl bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  argTypes: {
    title: {
      control: 'text',
    },
    description: {
      control: 'text',
    },
    linkLabel: {
      control: 'text',
    },
    linkUrl: {
      control: 'text',
    },
  },
  args: {
    title: 'Paiements récents',
    description: 'Les derniers encaissements qui ont fait bouger le mois.',
    linkLabel: 'Historique',
    linkUrl: '#',
  },
};

export default meta;

type Story = StoryObj<DashboardCard>;

export const RecentPayments: Story = {
  render: args => ({
    props: args,
    template: `
      <ui-dashboard-card
        [title]="title"
        [description]="description"
        [linkLabel]="linkLabel"
        [linkUrl]="linkUrl"
      >
        <div class="space-y-3">
          <div class="flex items-center justify-between rounded-lg border border-base-200 bg-slate-50 px-4 py-3">
            <div>
              <p class="font-semibold text-slate-900">Sophie Martin</p>
              <p class="text-sm text-slate-500">Lot A01 · 26 avril 2026</p>
            </div>
            <span class="font-semibold text-slate-900">680 €</span>
          </div>
          <div class="flex items-center justify-between rounded-lg border border-base-200 bg-slate-50 px-4 py-3">
            <div>
              <p class="font-semibold text-slate-900">Mathieu Leroy</p>
              <p class="text-sm text-slate-500">Lot B03 · 24 avril 2026</p>
            </div>
            <span class="font-semibold text-slate-900">540 €</span>
          </div>
        </div>
      </ui-dashboard-card>
    `,
  }),
};

export const WithoutLink: Story = {
  args: {
    title: 'Occupation des lots',
    description: 'La santé actuelle du parc locatif.',
    linkLabel: undefined,
  },
  render: args => ({
    props: args,
    template: `
      <ui-dashboard-card [title]="title" [description]="description" [linkLabel]="linkLabel">
        <div class="stats stats-vertical rounded-lg border border-base-200 bg-slate-50 lg:stats-horizontal">
          <div class="stat">
            <div class="stat-title">Lots occupés</div>
            <div class="stat-value text-success">7</div>
          </div>
          <div class="stat">
            <div class="stat-title">Lots vacants</div>
            <div class="stat-value text-warning">2</div>
          </div>
          <div class="stat">
            <div class="stat-title">Potentiel mensuel</div>
            <div class="stat-value text-slate-900">5 080 €</div>
          </div>
        </div>
      </ui-dashboard-card>
    `,
  }),
};
