import { Component, inject } from '@angular/core';
import { Meta } from '@angular/platform-browser';
import { ContactDesign } from '../components/ContactDesign/contact-design';

@Component({
  selector: 'app-contact-page',
  imports: [ContactDesign],
  template: `<app-contact-design></app-contact-design>`,
  styles: ``,
})
export class ContactPage {
    private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'Contact us, reach us, get in touch'
    });
  }


}
