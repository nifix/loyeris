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
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [FormField],
    }),
    componentWrapperDecorator(
      story => `
        <div data-theme="corporate" class="inline-block bg-base-200 p-6">
          <div class="w-96 max-w-[calc(100vw-2rem)] rounded-lg border border-base-200/60 bg-base-100 p-6 shadow-xl">
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
    required: {
      control: 'boolean',
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

export const Email: Story = {
  args: {
    autocomplete: 'email',
    fieldId: 'email',
    label: 'Adresse e-mail',
    placeholder: 'nom@exemple.fr',
    type: 'email',
  },
};

export const Password: Story = {
  args: {
    autocomplete: 'current-password',
    fieldId: 'password',
    label: 'Mot de passe',
    placeholder: 'Votre mot de passe',
    forgotLabel: 'Mot de passe oublié ?',
    type: 'password',
  },
};

export const Name: Story = {
  args: {
    autocomplete: 'given-name',
    fieldId: 'first-name',
    label: 'Prénom',
    placeholder: 'Camille',
    required: true,
    type: 'text',
  },
};

export const LoginForm: Story = {
  render: () => ({
    template: `
      <form class="space-y-5">
        <ui-form-field
          fieldId="email"
          label="Adresse email"
          type="email"
          autocomplete="email"
          placeholder="nom@exemple.com"
        />

        <ui-form-field
          fieldId="password"
          label="Mot de passe"
          type="password"
          autocomplete="current-password"
          placeholder="********"
          forgotLabel="Mot de passe oublié ?"
        />
      </form>
    `,
  })
};

