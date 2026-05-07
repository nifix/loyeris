import { componentWrapperDecorator, type Meta, type StoryObj } from '@storybook/angular';

import { BrandMark } from './brand-mark';

const meta: Meta<BrandMark> = {
  title: 'Features/Login/Components/Brand Mark',
  component: BrandMark,
  tags: ['autodocs'],
  decorators: [
    componentWrapperDecorator(
      story => `
        <div class="grid min-h-48 place-items-center bg-[#0a1628] p-8 text-white">
          ${story}
        </div>
      `,
    ),
  ],
  argTypes: {
    size: {
      control: 'radio',
      options: ['sm', 'lg'],
    },
  },
  args: {
    size: 'lg',
  },
};

export default meta;

type Story = StoryObj<BrandMark>;

export const Large: Story = {};

export const Small: Story = {
  args: {
    size: 'sm',
  },
};

export const Sizes: Story = {
  render: () => ({
    template: `
      <div class="flex items-center gap-5">
        <app-brand-mark size="lg" />
        <app-brand-mark size="sm" />
      </div>
    `,
  }),
};

