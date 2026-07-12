import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { ScisPage } from './scis-page';

const meta: Meta<ScisPage> = {
  title: 'Features/SCIs/Pages/SCI List',
  component: ScisPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<ScisPage>;

const sampleSci = {
  id: 'sci-1',
  workspaceId: 'workspace-1',
  name: 'SCI Les Tilleuls',
  siren: '123456789',
  taxRegime: 'IR' as const,
  status: 'Active' as const,
  street: '12 rue des Tilleuls',
  postalCode: '69000',
  city: 'Lyon',
  country: 'FR',
  incorporatedOn: '2024-01-10',
  createdAt: '2026-07-12T08:00:00Z',
  updatedAt: '2026-07-12T08:00:00Z',
  archivedAt: null,
};

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

const portfolioArgs = {
  presentationScis: [sampleSci],
  presentationLots: [sampleLot],
  presentationOccupancies: [sampleOccupancy],
  presentationState: 'loaded' as const,
};

export const Default: Story = {
  args: portfolioArgs,
};

export const WithArchivedStructures: Story = {
  args: {
    ...portfolioArgs,
    presentationScis: [
      sampleSci,
      {
        ...sampleSci,
        id: 'sci-archived',
        name: 'SCI Carnot',
        status: 'Archived',
        archivedAt: '2026-06-30T08:00:00Z',
      },
    ],
  },
};

export const Loading: Story = {
  args: { presentationScis: [], presentationState: 'loading' },
};

export const Empty: Story = {
  args: { presentationScis: [], presentationState: 'loaded' },
};

export const Error: Story = {
  args: { presentationScis: [], presentationState: 'error' },
};

export const Created: Story = {
  args: {
    ...portfolioArgs,
    creationSucceeded: true,
  },
};

export const Updated: Story = {
  args: {
    ...portfolioArgs,
    updateSucceeded: true,
  },
};

function currentMonthDate(day: number): string {
  const currentDate = new Date();
  const year = currentDate.getFullYear();
  const month = String(currentDate.getMonth() + 1).padStart(2, '0');
  return `${year}-${month}-${String(day).padStart(2, '0')}`;
}
