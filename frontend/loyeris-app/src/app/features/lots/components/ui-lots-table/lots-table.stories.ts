import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { LotsTable, type LotListItem } from './lots-table';

const lots: readonly LotListItem[] = [
  {
    reference: 'Lot A01',
    sci: 'SCI Les Tilleuls',
    address: '12 rue des Tilleuls, Lyon',
    type: 'T2',
    occupation: 'Sophie Martin',
    rent: '680 €',
    status: 'Occupé',
    statusTone: 'success',
    detailUrl: '#',
  },
  {
    reference: 'Lot A03',
    sci: 'SCI Les Tilleuls',
    address: '12 rue des Tilleuls, Lyon',
    type: 'Garage',
    occupation: '—',
    rent: '90 €',
    status: 'Vacant',
    statusTone: 'warning',
    detailUrl: '#',
  },
  {
    reference: 'Lot B01',
    sci: 'SCI Carnot',
    address: '8 boulevard Carnot, Villeurbanne',
    type: 'T1',
    occupation: 'Louis Mercier',
    rent: '540 €',
    status: 'Occupé',
    statusTone: 'success',
    detailUrl: '#',
  },
];

const meta: Meta<LotsTable> = {
  title: 'Features/Lots/Components/Lots Table',
  component: LotsTable,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [LotsTable],
    }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[72rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: {
    lots,
  },
};

export default meta;

type Story = StoryObj<LotsTable>;

export const Default: Story = {};

export const OccupiedOnly: Story = {
  args: {
    lots: lots.filter((lot) => lot.statusTone === 'success'),
  },
};
