import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { LoginPage } from './login-page';

const meta: Meta<LoginPage> = {
  title: 'Features/Login/Pages/Login',
  component: LoginPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<LoginPage>;

export const Default: Story = {
  args: {
    submissionErrorCode: null,
  },
};

export const InvalidCredentials: Story = {
  args: {
    submissionErrorCode: 'invalid-credentials',
  },
};

export const UnknownAccount: Story = {
  name: 'Unknown account (neutral message)',
  args: {
    submissionErrorCode: 'account-not-found',
  },
};

export const EmailNotVerified: Story = {
  args: {
    submissionErrorCode: 'email-not-verified',
  },
};

export const GenericError: Story = {
  args: {
    submissionErrorCode: 'generic',
  },
};
