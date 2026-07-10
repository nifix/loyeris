import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { RentsTable, type RentListItem } from './rents-table';

const rents: readonly RentListItem[] = [
  {
    tenant: 'Sophie Martin', lot: 'Lot A01', sci: 'SCI Les Tilleuls', due: '680 €', paid: '680 €', remaining: '0 €', paymentDate: '26/04/2026', status: 'Payé', statusTone: 'success', action: 'Voir →', actionTone: 'ghost', actionUrl: '#',
  },
  {
    tenant: 'Camille Robert', lot: 'Lot A02', sci: 'SCI Les Tilleuls', due: '620 €', paid: '0 €', remaining: '620 €', paymentDate: '—', status: 'En retard', statusTone: 'error', action: 'Encaisser', actionTone: 'primary', actionUrl: '#',
  },
  {
    tenant: 'Louis Mercier', lot: 'Lot B01', sci: 'SCI Carnot', due: '540 €', paid: '380 €', remaining: '160 €', paymentDate: '21/04/2026', status: 'Partiel', statusTone: 'warning', action: 'Compléter', actionTone: 'warning', actionUrl: '#',
  },
];

const meta: Meta<RentsTable> = {
  title: 'Features/Rents/Components/Rents Table',
  component: RentsTable,
  tags: ['autodocs'],
  parameters: { layout: 'centered' },
  decorators: [
    moduleMetadata({ imports: [RentsTable] }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[80rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: { rents, period: 'Avril 2026' },
};

export default meta;

type Story = StoryObj<RentsTable>;

export const Default: Story = {};

export const LatePayments: Story = {
  args: { rents: rents.filter((rent) => rent.statusTone !== 'success') },
};
