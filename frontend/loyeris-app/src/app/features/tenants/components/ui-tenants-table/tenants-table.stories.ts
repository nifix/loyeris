import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { TenantsTable, type TenantListItem } from './tenants-table';

const tenants: readonly TenantListItem[] = [
  {
    name: 'Sophie Martin',
    sci: 'SCI Les Tilleuls',
    currentLot: 'Lot A01',
    entryDate: '15/09/2025',
    email: 'sophie.martin@email.fr',
    phone: '06 11 22 33 44',
    status: 'Actif',
    statusTone: 'success',
    detailUrl: '#',
  },
  {
    name: 'Camille Robert',
    sci: 'SCI Les Tilleuls',
    currentLot: 'Lot A02',
    entryDate: '01/02/2026',
    email: 'camille.robert@email.fr',
    phone: '06 22 33 44 55',
    status: 'Actif',
    statusTone: 'success',
    detailUrl: '#',
  },
  {
    name: 'Élodie Marchal',
    sci: 'SCI Les Tilleuls',
    currentLot: '—',
    entryDate: '01/05/2024',
    email: 'elodie.marchal@email.fr',
    phone: '06 44 55 66 77',
    status: 'Parti',
    statusTone: 'neutral',
    detailUrl: '#',
  },
];

const meta: Meta<TenantsTable> = {
  title: 'Features/Tenants/Components/Tenants Table',
  component: TenantsTable,
  tags: ['autodocs'],
  parameters: { layout: 'centered' },
  decorators: [
    moduleMetadata({ imports: [TenantsTable] }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[72rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: { tenants },
};

export default meta;

type Story = StoryObj<TenantsTable>;

export const Default: Story = {};

export const ActiveOnly: Story = {
  args: { tenants: tenants.filter((tenant) => tenant.statusTone === 'success') },
};
