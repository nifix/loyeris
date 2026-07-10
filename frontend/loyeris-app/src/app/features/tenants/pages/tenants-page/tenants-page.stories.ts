import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { TenantsPage } from './tenants-page';

const meta: Meta<TenantsPage> = {
  title: 'Features/Tenants/Pages/Tenant List',
  component: TenantsPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<TenantsPage>;

export const Default: Story = {};
