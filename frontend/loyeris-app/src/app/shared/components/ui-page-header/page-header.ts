import { Component, input } from '@angular/core';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'ui-page-header',
  imports: [RouterLink],
  templateUrl: './page-header.html',
  styleUrl: './page-header.css',
})
export class PageHeader {
  readonly badge = input.required<string>();
  readonly title = input.required<string>();
  readonly description = input.required<string>();
  readonly chips = input<readonly string[]>([]);
  readonly actionLabel = input<string>();
  readonly actionUrl = input<string>();
}
