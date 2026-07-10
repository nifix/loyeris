import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { TenantDetailPage } from './tenant-detail-page';

const meta: Meta<TenantDetailPage> = {
  title: 'Features/Tenants/Pages/Tenant Detail',
  component: TenantDetailPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<TenantDetailPage>;

export const Default: Story = {};
