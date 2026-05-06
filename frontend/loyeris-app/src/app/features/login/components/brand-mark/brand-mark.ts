import { Component, input } from '@angular/core';

@Component({
  selector: 'app-brand-mark',
  templateUrl: './brand-mark.html',
  styleUrl: './brand-mark.css',
})

export class BrandMark {
  readonly size = input<'sm' | 'lg'>('lg');
}
