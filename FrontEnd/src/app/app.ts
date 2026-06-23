import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: `
   

    <router-outlet />
  `,
  styles: [],
})
export class App {
  protected readonly title = signal('FrontEnd');
}

// Kako da za specificnu rutu vratim odredjenu componentu?
// - Kako ide sintaksa da napravim samo .ts fajl?

// Kako cu organizovati strukturu projekta?
// Koje foldere cu imati?
// - layouts
// - common
// - pages
// 

// Kako da sada namestim da se ucita home componenta kada se pozove / ruta