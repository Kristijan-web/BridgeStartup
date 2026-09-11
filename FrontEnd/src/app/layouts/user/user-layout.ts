import { Component } from '@angular/core';
import { Header } from './static/header';
import { RouterOutlet } from '@angular/router';
import { Footer } from './static/footer';

@Component({
  selector: 'app-user',
  imports: [Header, RouterOutlet, Footer],
  template: `
  
    <div class="flex min-h-screen flex-col">

        <app-header />

        <main class="flex-1">
          <router-outlet />
        </main>

       <app-footer />

    </div>

             
          
  `,
  styles: ``,
})
export class UserLayout {}


