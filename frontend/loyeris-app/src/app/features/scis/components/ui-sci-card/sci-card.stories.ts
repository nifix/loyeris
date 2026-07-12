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
  args: {
    id: 'sci-1',
    name: 'SCI Les Tilleuls',
    status: 'Active',
    address: '12 rue des Tilleuls, Lyon',
    createdAt: '14 janvier 2024',
    incorporatedOn: '06/12/2023',
    siren: '123456789',
    taxRegime: 'IR',
  },
};

export default meta;

type Story = StoryObj<SciCard>;

export const Default: Story = {};

export const ArchivedWithoutLegalDetails: Story = {
  args: {
    name: 'SCI Carnot',
    status: 'Archived',
    address: '8 boulevard Carnot, Villeurbanne',
    createdAt: '3 mars 2025',
    incorporatedOn: undefined,
    siren: undefined,
    taxRegime: 'IR',
  },
};
