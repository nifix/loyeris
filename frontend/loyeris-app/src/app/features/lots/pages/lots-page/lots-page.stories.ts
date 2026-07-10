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

export const Default: Story = {};
