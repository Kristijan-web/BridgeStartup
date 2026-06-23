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


// Sta sad treba da uradim?
// - Da dodam head i body tagove, deo head-a treba dinamicki da se ucitava

// Koji deo <head> taga dinamicki dobija vrednost?
// - <title></title>
// - keywords

// Kako da se title unutar <head> taga promeni kada se promeni stranica?

// head tag je unutar index.html
