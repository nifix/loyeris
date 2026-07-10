import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { LotDetailPage } from './lot-detail-page';

const meta: Meta<LotDetailPage> = {
  title: 'Features/Lots/Pages/Lot Detail',
  component: LotDetailPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<LotDetailPage>;

export const Default: Story = {};
