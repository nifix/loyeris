import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { StatusBadge } from './status-badge';

const meta: Meta<StatusBadge> = {
  title: 'Shared/Components/Status Badge',
  component: StatusBadge,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [StatusBadge],
    }),
    componentWrapperDecorator(
      story => `<div data-theme="corporate" class="inline-block bg-base-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  argTypes: {
    tone: {
      control: 'radio',
      options: ['success', 'warning', 'error', 'neutral'],
    },
  },
  args: {
    tone: 'success',
  },
};

export default meta;

type Story = StoryObj<StatusBadge>;

export const Default: Story = {
  render: args => ({
    props: args,
    template: `<ui-status-badge [tone]="tone">Actif</ui-status-badge>`,
  }),
};

export const States: Story = {
  render: () => ({
    template: `
      <div class="flex flex-wrap items-center gap-3">
        <ui-status-badge tone="success">Payé</ui-status-badge>
        <ui-status-badge tone="warning">Partiel</ui-status-badge>
        <ui-status-badge tone="error">En retard</ui-status-badge>
        <ui-status-badge tone="neutral">Parti</ui-status-badge>
      </div>
    `,
  }),
};
