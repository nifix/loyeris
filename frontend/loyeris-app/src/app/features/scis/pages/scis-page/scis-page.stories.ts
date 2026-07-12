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

export const Default: Story = {
  args: { presentationScis: [sampleSci], presentationState: 'loaded' },
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
    presentationScis: [sampleSci],
    presentationState: 'loaded',
    creationSucceeded: true,
  },
};

export const Updated: Story = {
  args: {
    presentationScis: [sampleSci],
    presentationState: 'loaded',
    updateSucceeded: true,
  },
};
