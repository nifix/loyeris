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

export const Default: Story = {};
