import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { SciCard } from './sci-card';

const meta: Meta<SciCard> = {
  title: 'Features/SCI/Components/SCI Card',
  component: SciCard,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [SciCard],
    }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[72rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  argTypes: {
    performanceTone: {
      control: 'radio',
      options: ['primary', 'success'],
    },
  },
  args: {
    name: 'SCI Les Tilleuls',
    status: 'Active',
    address: '12 rue des Tilleuls, Lyon',
    createdAt: '14 janvier 2024',
    lots: 5,
    tenants: 4,
    monthlyRent: '2 760 €',
    occupancy: '80%',
    performance: 79,
    performanceTone: 'primary',
  },
};

export default meta;

type Story = StoryObj<SciCard>;

export const Default: Story = {};

export const StrongPerformance: Story = {
  args: {
    name: 'SCI Carnot',
    address: '8 boulevard Carnot, Villeurbanne',
    createdAt: '3 mars 2025',
    lots: 4,
    tenants: 3,
    monthlyRent: '1 560 €',
    occupancy: '75%',
    performance: 87,
    performanceTone: 'success',
  },
};
