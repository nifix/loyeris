import { moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { FormField } from './form-field';

const meta: Meta<FormField> = {
  title: 'Features/Login/Components/FormField',
  component: FormField,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [FormField],
    }),
  ],
  argTypes: {
    type: {
      control: 'radio',
      options: ['text', 'email', 'password'],
    },
  },
  args: {
    autocomplete: 'email',
    fieldId: 'email',
    label: 'Adresse e-mail',
    placeholder: 'nom@exemple.fr',
    type: 'email',
  },
};

export default meta;

type Story = StoryObj<FormField>;

export const Email: Story = {};

export const PasswordWithForgotLink: Story = {
  args: {
    autocomplete: 'current-password',
    fieldId: 'password',
    label: 'Mot de passe',
    placeholder: 'Votre mot de passe',
    forgotLabel: 'Mot de passe oublie ?',
    type: 'password',
  },
};

export const WithIcon: Story = {
  render: args => ({
    props: args,
    template: `
      <app-form-field
        [autocomplete]="autocomplete"
        [fieldId]="fieldId"
        [label]="label"
        [placeholder]="placeholder"
        [type]="type"
      >
        <svg
          fieldIcon
          xmlns="http://www.w3.org/2000/svg"
          viewBox="0 0 24 24"
          fill="none"
          stroke="currentColor"
          stroke-width="2"
          stroke-linecap="round"
          stroke-linejoin="round"
          aria-hidden="true"
        >
          <path d="M4 4h16v16H4z" />
          <path d="m22 6-10 7L2 6" />
        </svg>
      </app-form-field>
    `,
  }),
};

