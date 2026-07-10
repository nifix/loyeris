import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { MetricCard } from './metric-card';

const meta: Meta<MetricCard> = {
  title: 'Shared/Components/Metric Card',
  component: MetricCard,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [MetricCard],
    }),
    componentWrapperDecorator(
      story => `<div data-theme="corporate" class="max-w-sm bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  argTypes: {
    title: {
      control: 'text',
    },
    value: {
      control: 'text',
    },
    description: {
      control: 'text',
    },
    tone: {
      control: 'radio',
      options: ['primary', 'success', 'warning', 'error', 'neutral'],
    },
  },
  args: {
    title: 'Loyers attendus',
    value: '4 320 €',
    description: 'Avril 2026',
    tone: 'primary',
  },
};

export default meta;

type Story = StoryObj<MetricCard>;

export const Default: Story = {};

export const DashboardSet: Story = {
  parameters: {
    layout: 'fullscreen',
  },
  render: () => ({
    template: `
      <div class="grid gap-4 md:grid-cols-2 xl:grid-cols-4">
        <ui-metric-card title="Loyers attendus" value="4 320 €" description="Avril 2026" tone="primary" />
        <ui-metric-card title="Loyers encaissés" value="3 540 €" description="82% du total attendu" tone="success" />
        <ui-metric-card title="Reste à recevoir" value="780 €" description="3 paiements à compléter" tone="warning" />
        <ui-metric-card title="Lots occupés" value="7 / 9" description="Taux d'occupation : 77%" tone="neutral" />
      </div>
    `,
  }),
  decorators: [
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-full min-w-[72rem] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
};
