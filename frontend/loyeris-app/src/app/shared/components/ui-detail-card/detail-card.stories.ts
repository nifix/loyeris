import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { DetailCard } from './detail-card';

const meta: Meta<DetailCard> = {
  title: 'Shared/Components/Detail Card',
  component: DetailCard,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({ imports: [DetailCard] }),
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="w-[40rem] max-w-[calc(100vw-2rem)] bg-slate-100 p-6 text-slate-900">${story}</div>`,
    ),
  ],
  args: {
    title: 'Coordonnées',
    description: 'Informations personnelles et moyens de contact.',
  },
  render: (args) => ({
    props: args,
    template: `
      <ui-detail-card [title]="title" [description]="description">
        <div class="info-pair">
          <div class="info-label">Nom complet</div>
          <div class="info-value">Camille Robert</div>
        </div>
        <div class="info-pair">
          <div class="info-label">Email</div>
          <div class="info-value">camille.robert@email.fr</div>
        </div>
      </ui-detail-card>
    `,
  }),
};

export default meta;

type Story = StoryObj<DetailCard>;

export const Default: Story = {};
