import { componentWrapperDecorator, type Meta, type StoryObj } from '@storybook/angular';

import { EmailVerificationMessage } from './email-verification-message';

const meta: Meta<EmailVerificationMessage> = {
  title: 'Features/Login/Components/Email Verification Message',
  component: EmailVerificationMessage,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    componentWrapperDecorator(
      (story) => `
        <div data-theme="corporate" class="w-[31rem] max-w-[calc(100vw-2rem)] rounded-3xl bg-base-100 p-10 shadow-xl">
          ${story}
        </div>
      `,
    ),
  ],
  argTypes: {
    status: {
      control: 'radio',
      options: ['success', 'error'],
    },
  },
};

export default meta;

type Story = StoryObj<EmailVerificationMessage>;

export const Success: Story = {
  args: {
    status: 'success',
  },
};

export const Error: Story = {
  args: {
    status: 'error',
  },
};
