import type { Meta, StoryObj } from '@storybook/angular';

import { BrandMark } from './brand-mark';

const meta: Meta<BrandMark> = {
  title: 'Features/Login/Components/BrandMark',
  component: BrandMark,
  tags: ['autodocs'],
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

