import { Component } from '@angular/core';
import { MyPostsDesign } from '../components/MyPostsDesign/my-posts-design';

@Component({
  selector: 'app-my-posts',
  imports: [MyPostsDesign],
  template: `<app-my-posts-design></app-my-posts-design>`,
  styles: ``,
})
export class MyPostsPage {}
