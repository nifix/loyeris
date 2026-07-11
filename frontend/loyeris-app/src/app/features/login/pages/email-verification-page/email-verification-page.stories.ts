import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { EmailVerificationPage } from './email-verification-page';

const meta: Meta<EmailVerificationPage> = {
  title: 'Features/Login/Pages/Verify Email',
  component: EmailVerificationPage,
  parameters: {
    layout: 'fullscreen',
    options: { layout: { showPanel: false } },
  },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<EmailVerificationPage>;

export const Pending: Story = {
  args: {
    verificationStatus: 'pending',
  },
};

export const Verifying: Story = {
  args: {
    verificationStatus: 'verifying',
  },
};

export const Success: Story = {
  args: {
    verificationStatus: 'success',
  },
};

export const Error: Story = {
  args: {
    verificationStatus: 'error',
  },
};
