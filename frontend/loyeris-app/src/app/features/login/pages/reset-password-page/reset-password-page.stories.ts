import { type Meta, type StoryObj } from '@storybook/angular';

import { fullPageStoryDecorators } from '../../../../shared/storybook/full-page-story-decorators';
import { ResetPasswordPage } from './reset-password-page';

const meta: Meta<ResetPasswordPage> = {
  title: 'Features/Login/Pages/Reset Password',
  component: ResetPasswordPage,
  parameters: { layout: 'fullscreen' },
  decorators: fullPageStoryDecorators,
};

export default meta;

type Story = StoryObj<ResetPasswordPage>;

export const Validating: Story = { args: { presentationState: 'validating' } };
export const Form: Story = { args: { presentationState: 'form' } };
export const Submitting: Story = { args: { presentationState: 'submitting' } };
export const Success: Story = { args: { presentationState: 'success' } };
export const Invalid: Story = { args: { presentationState: 'invalid' } };
export const ValidationError: Story = { args: { presentationState: 'validation-error' } };
export const SubmissionError: Story = { args: { presentationState: 'submission-error' } };
