import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { LotFormPage } from './lot-form-page';

const meta: Meta<LotFormPage> = {
  title: 'Features/Lots/Pages/Lot Form',
  component: LotFormPage,
  parameters: { layout: 'fullscreen' },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<LotFormPage>;

const sci = {
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

const tenant = {
  id: 'tenant-1',
  firstName: 'Camille',
  lastName: 'Robert',
  email: 'camille@example.fr',
  isCurrent: false,
};

export const Creation: Story = {
  args: { presentationScis: [sci], presentationTenants: [tenant] },
};

export const Editing: Story = {
  args: {
    presentationScis: [sci],
    presentationTenants: [{ ...tenant, isCurrent: true }],
    presentationLot: {
      id: 'lot-1',
      sciId: 'sci-1',
      sciName: 'SCI Les Tilleuls',
      reference: 'Lot A01',
      type: 'T2',
      status: 'Active',
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
    },
    presentationOccupancy: {
      lotId: 'lot-1',
      leaseId: 'lease-1',
      tenantId: 'tenant-1',
      tenantFirstName: 'Camille',
      tenantLastName: 'Robert',
      startsOn: '2026-07-01',
      endsOn: '2027-06-30',
      rentDueDay: 5,
      rentExcludingChargesCents: 65000,
      chargesCents: 5000,
      depositCents: 65000,
      paymentTerms: 'Virement mensuel',
      notes: null,
    },
  },
};

export const Submitting: Story = {
  args: { presentationScis: [sci], presentationTenants: [tenant], submitting: true },
};
