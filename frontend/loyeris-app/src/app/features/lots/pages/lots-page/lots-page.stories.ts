import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { LotsPage } from './lots-page';

const meta: Meta<LotsPage> = {
  title: 'Features/Lots/Pages/Lot List',
  component: LotsPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<LotsPage>;

const sampleLot = {
  id: 'lot-1',
  sciId: 'sci-1',
  sciName: 'SCI Les Tilleuls',
  reference: 'Lot A01',
  type: 'T2' as const,
  status: 'Active' as const,
  street: '12 rue des Tilleuls',
  postalCode: '69000',
  city: 'Lyon',
  country: 'FR',
  surfaceSqm: 42.5,
  potentialRentExcludingChargesCents: 65000,
  potentialChargesCents: 5000,
  suggestedDepositCents: 65000,
  notes: null,
  createdAt: '2026-07-12T08:00:00Z',
  updatedAt: '2026-07-12T08:00:00Z',
  archivedAt: null,
};

const sampleOccupancy = {
  lotId: 'lot-1',
  leaseId: 'lease-1',
  tenantId: 'tenant-1',
  tenantFirstName: 'Camille',
  tenantLastName: 'Robert',
  startsOn: currentMonthDate(1),
  endsOn: null,
  rentDueDay: 5,
  rentExcludingChargesCents: 65000,
  chargesCents: 5000,
  depositCents: 65000,
  paymentTerms: null,
  notes: null,
};

export const Default: Story = {
  args: {
    presentationLots: [sampleLot],
    presentationOccupancies: [sampleOccupancy],
    presentationState: 'loaded',
  },
};

export const Empty: Story = {
  args: { presentationLots: [], presentationOccupancies: [], presentationState: 'loaded' },
};

export const Loading: Story = {
  args: { presentationLots: [], presentationOccupancies: [], presentationState: 'loading' },
};

export const Error: Story = {
  args: { presentationLots: [], presentationOccupancies: [], presentationState: 'error' },
};

function currentMonthDate(day: number): string {
  const currentDate = new Date();
  const year = currentDate.getFullYear();
  const month = String(currentDate.getMonth() + 1).padStart(2, '0');
  return `${year}-${month}-${String(day).padStart(2, '0')}`;
}
