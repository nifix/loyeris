import { componentWrapperDecorator, moduleMetadata, type Meta, type StoryObj } from '@storybook/angular';

import { AppShell } from './app-shell';

const meta: Meta<AppShell> = {
  title: 'Core/Layouts/App Shell',
  component: AppShell,
  tags: ['autodocs'],
  decorators: [
    moduleMetadata({
      imports: [AppShell],
    }),
    componentWrapperDecorator(
      story => `<div data-theme="corporate" class="min-h-screen bg-slate-100 text-slate-900">${story}</div>`,
    ),
  ],
};

export default meta;

type Story = StoryObj<AppShell>;

export const DashboardContent: Story = {
  render: () => ({
    template: `
      <ui-app-shell>
        <section class="card rounded-lg border border-base-200 bg-base-100 shadow-sm">
          <div class="card-body">
            <div class="badge badge-primary badge-outline w-fit">Vue globale</div>
            <h1 class="text-3xl font-bold text-slate-900">Dashboard</h1>
            <p class="max-w-2xl text-sm leading-6 text-slate-500">
              Aperçu du contenu projeté dans le shell applicatif avec navigation latérale.
            </p>
          </div>
        </section>
      </ui-app-shell>
    `,
  }),
};
