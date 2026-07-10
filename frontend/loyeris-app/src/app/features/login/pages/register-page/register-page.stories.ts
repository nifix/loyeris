import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { RegisterPage } from './register-page';

const meta: Meta<RegisterPage> = {
  title: 'Features/Login/Pages/Register',
  component: RegisterPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<RegisterPage>;

export const Default: Story = {
  args: {
    submissionErrorCode: null,
  },
};

export const EmailAlreadyExists: Story = {
  args: {
    submissionErrorCode: 'email-exists',
  },
};

export const GenericError: Story = {
  args: {
    submissionErrorCode: 'generic',
  },
};
