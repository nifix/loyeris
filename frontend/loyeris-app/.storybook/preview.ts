import { componentWrapperDecorator, type Preview } from '@storybook/angular';

if (typeof document !== 'undefined') {
  document.documentElement.setAttribute('data-theme', 'corporate');
  document.body.setAttribute('data-theme', 'corporate');
  document.body.style.background = '#f8fafc';
  document.body.style.fontFamily =
    "'Plus Jakarta Sans', Inter, system-ui, -apple-system, sans-serif";
}

const preview: Preview = {
  decorators: [
    componentWrapperDecorator(
      story =>
        `<div data-theme="corporate" class="inline-block bg-slate-50 text-slate-900" style="font-family: 'Plus Jakarta Sans', Inter, system-ui, -apple-system, sans-serif;">${story}</div>`,
    ),
  ],
  parameters: {
    layout: 'fullscreen',
    docs: {
      codePanel: true
    },
    backgrounds: {
      default: 'loyeris',
      values: [
        { name: 'loyeris', value: '#f8fafc' },
        { name: 'white', value: '#ffffff' },
      ],
    },
    controls: {
      matchers: {
        color: /(background|color)$/i,
        date: /Date$/i,
      },
    },
  },
};

export default preview;
