import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { PageHeader } from './page-header';

const meta: Meta<PageHeader> = {
  title: 'Shared/Components/Page Header',
  component: PageHeader,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [PageHeader],
    }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[72rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  argTypes: {
    badge: {
      control: 'text',
    },
    title: {
      control: 'text',
    },
    description: {
      control: 'text',
    },
    chips: {
      control: 'object',
    },
    actionLabel: {
      control: 'text',
    },
  },
  args: {
    badge: 'Vue globale',
    title: 'Dashboard',
    description:
      "Vue globale des encaissements, des retards et du taux d'occupation pour garder le portefeuille locatif sous contrôle.",
    chips: ['2 SCI actives', '7 lots occupés', '3 actions prioritaires'],
    actionLabel: 'Enregistrer un paiement',
  },
};

export default meta;

type Story = StoryObj<PageHeader>;

export const Default: Story = {};

export const WithoutAction: Story = {
  args: {
    badge: 'Portefeuille',
    title: 'Lots',
    description:
      "Vue d'ensemble des logements et locaux avec une lecture rapide des statuts d'occupation.",
    chips: ['9 lots suivis', '7 occupés', '5 080 € de potentiel mensuel'],
    actionLabel: undefined,
  },
};
