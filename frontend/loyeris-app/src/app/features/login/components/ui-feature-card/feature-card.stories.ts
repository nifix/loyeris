import {
  componentWrapperDecorator,
  moduleMetadata,
  type Meta,
  type StoryObj,
} from '@storybook/angular';

import { FeatureCard } from './feature-card';

const meta: Meta<FeatureCard> = {
  title: 'Features/Login/Components/Feature Card',
  component: FeatureCard,
  tags: ['autodocs'],
  parameters: {
    layout: 'centered',
  },
  decorators: [
    moduleMetadata({
      imports: [FeatureCard],
    }),
    componentWrapperDecorator(
      story => `
        <div
          class="inline-block rounded-lg p-8 text-white"
          style="background: linear-gradient(145deg, #0a1628 0%, #12326e 52%, #1a5faa 100%);"
        >
          ${story}
        </div>
      `,
    ),
  ],
  argTypes: {
    header: {
      control: 'text',
    },
    title: {
      control: 'text',
    },
    description: {
      control: 'text',
    },
  },
  args: {
    header: 'Suivi locatif',
    title: 'Pilotez vos loyers sereinement',
    description:
      'Visualisez les échéances, les retards et les paiements de chaque lot depuis un tableau de bord unique.',
  },
};

export default meta;

type Story = StoryObj<FeatureCard>;

const renderFeatureCard: Story['render'] = args => ({
  props: args,
  template: `
    <ui-feature-card
      [header]="header"
      [title]="title"
      [description]="description"
    />
  `,
});

export const Default: Story = {
  render: renderFeatureCard,
  decorators: [
    componentWrapperDecorator(story => `<div class="mx-auto max-w-sm">${story}</div>`),
  ],
};

export const Alerts: Story = {
  render: renderFeatureCard,
  decorators: [
    componentWrapperDecorator(story => `<div class="mx-auto max-w-sm">${story}</div>`),
  ],
  args: {
    header: 'Alertes intelligentes',
    title: 'Ne manquez aucune relance',
    description:
      'Recevez des rappels automatiques pour chaque loyer impayé et gardez une trace de vos actions de suivi.',
  },
};

export const LoginSet: Story = {
  parameters: {
    layout: 'centered',
  },
  render: () => ({
    template: `
      <div class="grid gap-4 sm:grid-cols-3">
        <ui-feature-card
          header="Multi-SCI"
          title="Tout sous un compte"
          description="Conservez une vue portefeuille tout en gardant le détail par structure."
        />
        <ui-feature-card
          header="Suivi locatif"
          title="Lots et occupants"
          description="Historique, occupation actuelle et accès rapide aux fiches détail."
        />
        <ui-feature-card
          header="Encaissements"
          title="Alertes et retards"
          description="Visualisez tout de suite ce qui est payé, partiel ou à relancer."
        />
      </div>
    `,
  }),
  decorators: [
    componentWrapperDecorator(
      story => `
        <div class="mx-auto max-w-5xl">
          ${story}
        </div>
      `,
    ),
  ],
};

