import { Component } from '@angular/core';
import { Header } from './static/header';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-user',
  imports: [Header, RouterOutlet],
  template: ` <app-header></app-header>
               <router-outlet></router-outlet>
  `,
  styles: ``,
})
export class UserLayout {}
