import { Component, input } from '@angular/core';

@Component({
  selector: 'app-badge-item',
  imports: [],
  template: ` <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">{{badgeName()}}</span> `,
  styles: ``,
})
export class BadgeItem {

  
  badgeName = input.required<string>();

}
