import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { SettingsPage } from './settings-page';

const meta: Meta<SettingsPage> = {
  title: 'Features/Settings/Pages/Settings',
  component: SettingsPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<SettingsPage>;

export const Default: Story = {};
