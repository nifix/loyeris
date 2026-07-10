import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { DetailPageHeader } from './detail-page-header';

const meta: Meta<DetailPageHeader> = {
  title: 'Shared/Components/Detail Page Header',
  component: DetailPageHeader,
  tags: ['autodocs'],
  parameters: { layout: 'centered' },
  decorators: [
    moduleMetadata({ imports: [DetailPageHeader] }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[72rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: {
    badge: 'Fiche locataire',
    status: 'Actif',
    statusTone: 'success',
    title: 'Camille Robert',
    description: 'Locataire actif · SCI Les Tilleuls · Lot A02',
    chips: ['Entrée le 1 février 2026', '620 € charges comprises', '1 retard en cours'],
    secondaryActionLabel: 'Modifier',
    primaryActionLabel: 'Voir les loyers',
    primaryActionUrl: '#',
  },
};

export default meta;

type Story = StoryObj<DetailPageHeader>;

export const Default: Story = {};
