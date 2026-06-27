import { Component, input } from '@angular/core';

@Component({
  selector: 'app-badge-item',
  imports: [],
  template: ` <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">{{ badgeName() }}</span> `,
  styles: ``,
})
export class BadgeItem {
  // Kako definisem prop
  // - naziv propa
  // - input objekat
  // - Tipiziranje inputa

  
  badgeName = input.required<string>();

}

// Sta sad treba da se uradi
// - Componenta koja prosledjuje prop treba da prosledi naziv badgeva
// Ja bih napravio PostBadges servis, koji ce da kada se prosledi id post-a, dohvatiti niz objekata gde ce svaki objekat biti 1 badge za taj post
// Koja componenta bi pozvala servis PostBadges?
// - posts.ts
// - post-details-design.cs

// Kakav je sada proces?
// - Napravi servis PostBadgesServicce
// - Definisu logiku da se prosledi specifican post i dohvate njegovi badge-evi
// - Neka componente posts.cs i post-details-design.cs pozovu servis

// Da li ima potrebe da pravis servis, pa 2 componeneta na razlicitm ssranicama ce trebati badge-ovi za specifican post, tako da da treba