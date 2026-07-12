import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { LotsTable, type LotListItem } from './lots-table';

const lots: readonly LotListItem[] = [
  {
    id: 'lot-a01',
    reference: 'Lot A01',
    sci: 'SCI Les Tilleuls',
    address: '12 rue des Tilleuls, Lyon',
    type: 'T2',
    occupation: 'Sophie Martin',
    amount: '730,00 €',
    amountBreakdown: '680,00 € HC + 50,00 € charges',
    archived: false,
    hasReceivableAmount: true,
    status: 'Occupé',
    statusTone: 'success',
    editUrl: '/lots/lot-a01',
  },
  {
    id: 'lot-a02',
    reference: 'Lot A03',
    sci: 'SCI Les Tilleuls',
    address: '12 rue des Tilleuls, Lyon',
    type: 'Garage',
    occupation: '—',
    amount: '—',
    amountBreakdown: '',
    archived: false,
    hasReceivableAmount: false,
    status: 'Vacant',
    statusTone: 'warning',
    editUrl: '/lots/lot-a02',
  },
  {
    id: 'lot-a03',
    reference: 'Lot B01',
    sci: 'SCI Carnot',
    address: '8 boulevard Carnot, Villeurbanne',
    type: 'T1',
    occupation: 'Louis Mercier',
    amount: '590,00 €',
    amountBreakdown: '540,00 € HC + 50,00 € charges',
    archived: false,
    hasReceivableAmount: true,
    status: 'Occupé',
    statusTone: 'success',
    editUrl: '/lots/lot-a03',
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
