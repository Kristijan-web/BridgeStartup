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


// Sada treba da asinhrono dohvatim podatke

// Da li podatke za post da cuvam globalno ili na nivou componente?
// -  Posto ce mi vrv trebati podaci na vise componenti kroz aplikaciju, mozda ce mi trebati da prikazem broj ukupnih proizvoda itd..., onda je bolje ici globalno
