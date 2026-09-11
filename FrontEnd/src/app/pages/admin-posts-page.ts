import { Component } from '@angular/core';
import { AdminPostsDesign } from '../components/AdminPostsDesign/admin-posts-design';

@Component({
  selector: 'app-admin-posts',
  imports: [AdminPostsDesign],
  template: `<app-admin-posts-design></app-admin-posts-design>`,
  styles: ``,
})
export class AdminPostsPage {}
