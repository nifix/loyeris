import {
  componentWrapperDecorator,
  moduleMetadata,
  type Meta,
  type StoryObj,
} from '@storybook/angular';

import { FormField } from './form-field';

const meta: Meta<FormField> = {
  title: 'Features/Login/Components/Form Field',
  component: FormField,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [FormField],
    }),
    componentWrapperDecorator(
      story => `
        <div class="min-h-56 bg-base-200 p-8">
          <div class="mx-auto max-w-md rounded-3xl border border-base-200/60 bg-base-100 p-8 shadow-xl">
            ${story}
          </div>
        </div>
      `,
    ),
  ],
  argTypes: {
    autocomplete: {
      control: 'text',
    },
    fieldId: {
      control: 'text',
    },
    forgotLabel: {
      control: 'text',
    },
    label: {
      control: 'text',
    },
    placeholder: {
      control: 'text',
    },
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

const emailIcon = `
  <svg
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-linecap="round"
    stroke-linejoin="round"
    stroke-width="2"
    aria-hidden="true"
  >
    <rect width="20" height="16" x="2" y="4" rx="2" />
    <path d="m22 7-8.97 5.7a1.94 1.94 0 0 1-2.06 0L2 7" />
  </svg>
`;

const passwordIcon = `
  <svg
    xmlns="http://www.w3.org/2000/svg"
    viewBox="0 0 24 24"
    fill="none"
    stroke="currentColor"
    stroke-linecap="round"
    stroke-linejoin="round"
    stroke-width="2"
    aria-hidden="true"
  >
    <rect width="18" height="11" x="3" y="11" rx="2" ry="2" />
    <path d="M7 11V7a5 5 0 0 1 10 0v4" />
  </svg>
`;

const renderWithIcon = (icon: string): Story['render'] => args => ({
  props: args,
  template: `
    <app-form-field
      [autocomplete]="autocomplete"
      [fieldId]="fieldId"
      [forgotLabel]="forgotLabel"
      [label]="label"
      [placeholder]="placeholder"
      [type]="type"
    >
      ${icon}
    </app-form-field>
  `,
});

export const Email: Story = {
  render: renderWithIcon(emailIcon),
};

export const Password: Story = {
  render: renderWithIcon(passwordIcon),
  args: {
    autocomplete: 'current-password',
    fieldId: 'password',
    label: 'Mot de passe',
    placeholder: 'Votre mot de passe',
    forgotLabel: 'Mot de passe oublié ?',
    type: 'password',
  },
};

export const LoginForm: Story = {
  render: () => ({
    template: `
      <form class="space-y-5">
        <app-form-field
          fieldId="email"
          label="Adresse email"
          type="email"
          autocomplete="email"
          placeholder="nom@exemple.com"
        >
          ${emailIcon}
        </app-form-field>

        <app-form-field
          fieldId="password"
          label="Mot de passe"
          type="password"
          autocomplete="current-password"
          placeholder="********"
          forgotLabel="Mot de passe oublié ?"
        >
          ${passwordIcon}
        </app-form-field>
      </form>
    `,
  })
};

