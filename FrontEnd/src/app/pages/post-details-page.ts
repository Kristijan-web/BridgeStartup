import { Component, inject } from '@angular/core';
import { PostDetailsDesign } from '../components/PostDetailsDesign/post-details-design';
import { Meta } from '@angular/platform-browser';

@Component({
  selector: 'app-post-details-page',
  imports: [PostDetailsDesign],
  template: ` <app-post-details-design/>  `,
  styles: ``,
})
export class PostDetailsPage {

   private meta = inject(Meta);

  constructor() {
    this.meta.updateTag({
      name: 'description',
      content: 'Find more about the specific post, get more details'
    });
  }

}
  // url = `http://localhost:3000/posts`;

