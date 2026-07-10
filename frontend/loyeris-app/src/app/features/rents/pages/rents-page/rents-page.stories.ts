import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { RentsPage } from './rents-page';

const meta: Meta<RentsPage> = {
  title: 'Features/Rents/Pages/Rent Tracking',
  component: RentsPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<RentsPage>;

export const Default: Story = {};
