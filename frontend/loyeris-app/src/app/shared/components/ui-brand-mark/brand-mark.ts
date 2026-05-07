import { Component, input } from '@angular/core';

@Component({
  selector: 'ui-brand-mark',
  templateUrl: './brand-mark.html',
  styleUrl: './brand-mark.css',
})
export class BrandMark {
  readonly size = input<'sm' | 'lg'>('lg');
}
