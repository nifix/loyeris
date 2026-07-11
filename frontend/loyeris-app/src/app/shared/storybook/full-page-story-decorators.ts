import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { applicationConfig, componentWrapperDecorator } from '@storybook/angular';

export const fullPageStoryDecorators = [
  applicationConfig({
    providers: [provideHttpClient(), provideRouter([{ path: '**', children: [] }])],
  }),
  componentWrapperDecorator(
    (story) => `<div data-theme="corporate" class="block min-h-screen w-screen">${story}</div>`,
  ),
];
