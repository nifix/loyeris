import type { Meta, StoryObj } from '@storybook/angular';

import { FeatureCard } from './feature-card';

const meta: Meta<FeatureCard> = {
  title: 'Features/Login/Components/FeatureCard',
  component: FeatureCard,
  tags: ['autodocs'],
  args: {
    header: 'Suivi locatif',
    title: 'Pilotez vos loyers sereinement',
    description:
      'Visualisez les echeances, les retards et les paiements de chaque lot depuis un tableau de bord unique.',
  },
};

export default meta;

type Story = StoryObj<FeatureCard>;

export const Default: Story = {};

export const AlertingFocus: Story = {
  args: {
    header: 'Alertes intelligentes',
    title: 'Ne manquez aucune relance',
    description:
      'Recevez des rappels automatiques pour chaque loyer impaye et gardez une trace de vos actions de suivi.',
  },
};

