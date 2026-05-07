import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-page-header',
  templateUrl: './page-header.html',
  styleUrl: './page-header.css',
})
export class PageHeader {
  readonly badge = input.required<string>();
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly chips = input<readonly string[]>([]);
  readonly actionLabel = input<string>();
}
