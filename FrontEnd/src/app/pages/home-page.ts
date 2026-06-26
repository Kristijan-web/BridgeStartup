import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { HomeDesign } from '../components/HomeDesign/home-design';

// Sta hocu da uradim?
// - Prvo napravim componentu Post-item koja ce bidi pozvavna u foreach-u

@Component({
  selector: 'app-home-page',
  imports: [HomeDesign],
  template: `  
        <app-home-design></app-home-design>
   `,
  styles: ``,
})
export class HomePage {
    private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'Browse posts, projects, and opportunities on the BridgeStartup platform.'
    });
  }
}
