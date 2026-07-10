import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { DashboardPage } from './dashboard-page';

const meta: Meta<DashboardPage> = {
  title: 'Features/Dashboard/Pages/Dashboard',
  component: DashboardPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<DashboardPage>;

export const Default: Story = {};
