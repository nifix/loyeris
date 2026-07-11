import { componentWrapperDecorator, type Meta, type StoryObj } from '@storybook/angular';

import { PasswordChecklist } from './password-checklist';

const meta: Meta<PasswordChecklist> = {
  title: 'Features/Login/Components/Password Checklist',
  component: PasswordChecklist,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    componentWrapperDecorator(
      (story) => `
        <div data-theme="corporate" class="w-[30rem] max-w-[calc(100vw-2rem)] bg-base-100 p-6">
          ${story}
        </div>
      `,
    ),
  ],
  argTypes: {
    password: {
      control: 'text',
    },
  },
  args: {
    password: '',
  },
};

export default meta;

type Story = StoryObj<PasswordChecklist>;

export const Default: Story = {};

export const PartiallyValid: Story = {
  args: {
    password: 'motdepasse',
  },
};

export const Valid: Story = {
  args: {
    password: 'Loyeris1!',
  },
};
