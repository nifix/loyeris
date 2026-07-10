import { componentWrapperDecorator, type Meta, type StoryObj } from '@storybook/angular';

import { AuthSubmissionError } from './auth-submission-error';

const meta: Meta<AuthSubmissionError> = {
  title: 'Features/Login/Components/Auth Submission Error',
  component: AuthSubmissionError,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    componentWrapperDecorator(
      (story) => `
        <div data-theme="corporate" class="w-[26rem] max-w-[calc(100vw-2rem)] bg-base-100 p-6">
          ${story}
        </div>
      `,
    ),
  ],
};

export default meta;

type Story = StoryObj<AuthSubmissionError>;

export const InvalidCredentials: Story = {
  args: {
    title: 'Identifiants incorrects',
    message: 'L’adresse email ou le mot de passe renseigné est incorrect.',
  },
};

export const EmailAlreadyExists: Story = {
  render: () => ({
    template: `
      <ui-auth-submission-error
        title="Cette adresse email est déjà utilisée"
        message="Un compte Loyeris est déjà associé à cette adresse."
      >
        <a
          auth-error-action
          class="link link-error mt-2 inline-flex text-sm font-semibold"
          href="/login"
        >
          Se connecter
        </a>
      </ui-auth-submission-error>
    `,
  }),
};

export const Generic: Story = {
  args: {
    title: 'Connexion impossible',
    message: 'Une erreur inattendue est survenue. Vérifiez votre connexion puis réessayez.',
  },
};
