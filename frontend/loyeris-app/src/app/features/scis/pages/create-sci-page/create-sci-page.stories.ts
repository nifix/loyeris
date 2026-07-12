import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { CreateSciPage } from './create-sci-page';

const meta: Meta<CreateSciPage> = {
  title: 'Features/SCIs/Pages/Create SCI',
  component: CreateSciPage,
  parameters: { layout: 'fullscreen' },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<CreateSciPage>;

const existingSci = {
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

export const Default: Story = {};
export const Editing: Story = { args: { editingSci: existingSci } };
export const Submitting: Story = { args: { submitting: true } };
export const DuplicateName: Story = { args: { submissionErrorCode: 'duplicate-name' } };
export const DuplicateSiren: Story = { args: { submissionErrorCode: 'duplicate-siren' } };
export const Forbidden: Story = { args: { submissionErrorCode: 'forbidden' } };
