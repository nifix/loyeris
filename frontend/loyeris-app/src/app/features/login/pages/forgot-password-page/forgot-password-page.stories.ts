import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { ForgotPasswordPage } from './forgot-password-page';

const meta: Meta<ForgotPasswordPage> = {
  title: 'Features/Login/Pages/Forgot Password',
  component: ForgotPasswordPage,
  parameters: { layout: 'fullscreen' },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<ForgotPasswordPage>;

export const Form: Story = { args: { presentationState: 'form' } };
export const Submitting: Story = { args: { presentationState: 'submitting' } };
export const Success: Story = { args: { presentationState: 'success' } };
export const Error: Story = { args: { presentationState: 'error' } };
