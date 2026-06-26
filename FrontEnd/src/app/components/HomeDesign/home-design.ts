import { Component, inject, signal } from '@angular/core';
import { PostsInterface } from '../../interfaces/posts-interface';
import { PostsService } from '../../services/posts-service';
import { Hero } from './sections/hero/hero';
import { Posts } from './sections/posts/posts';

@Component({
  selector: 'app-home-design',
  imports: [ Hero, Posts],

  template: ` 

          <app-hero></app-hero>

          <app-posts></app-posts>

      `,
  styles: ``,
})
export class HomeDesign {



}
