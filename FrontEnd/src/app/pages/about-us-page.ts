import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { AboutUsDesign } from '../components/AboutUsDesign/about-us-design';

@Component({
  selector: 'app-about-us-page',
  imports: [AboutUsDesign],
  template: `<app-about-us-design></app-about-us-design>`,
  styles: ``,
})
export class AboutUsPage {
  private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'About us, learn more, who are we'
    });
  }
}
