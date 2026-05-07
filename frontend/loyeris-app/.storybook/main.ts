import type { StorybookConfig } from '@storybook/angular/dist';

const config: StorybookConfig = {
  "stories": [
    '../src/**/*.mdx',
    "../src/**/*.stories.@(js|jsx|mjs|ts|tsx)"
  ],
  "addons": ['@storybook/addon-docs'],
  "framework": "@storybook/angular"
};
export default config;
