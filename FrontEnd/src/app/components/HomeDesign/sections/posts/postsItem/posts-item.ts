import { Component, inject, input, signal } from '@angular/core';
import { PostsInterface } from '../../../../../interfaces/posts-interface';
import { RouterLink } from '@angular/router';
import { PostsBadgeService } from '../../../../../services/posts-badge-service';
import { PostBadgesInterface } from '../../../../../interfaces/post-badges-interface';
import { BadgeItem } from '../../../../../common/badge-item';
import { BadgesInterface } from '../../../../../interfaces/badges-interface';
import { BadgesService } from '../../../../../services/badges-service';

// <span class="rounded-lg bg-indigo-50 px-3 py-1 text-xs font-black text-indigo-700">React</span>
//                             <span class="rounded-lg bg-cyan-50 px-3 py-1 text-xs font-black text-cyan-700">Node.js</span>
//                             <span class="rounded-lg bg-emerald-50 px-3 py-1 text-xs font-black text-emerald-700">PostgreSQL</span>
@Component({
  selector: 'app-post-item',
  imports: [RouterLink, BadgeItem],
  template: `<article class="flex min-h-[340px] flex-col rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
                        <div class="mb-4 flex flex-wrap gap-2">
                            // ovde idu dinamicki podaci
                            @for (badge of badges(); track badge.id) {
                                <app-badge-item [badgeName]="badge.name"></app-badge-item>
                            }


                        </div>
                        
                        <h3 class="text-xl font-black">{{ post().title }}</h3>
                        <p class="mt-3 flex-1 leading-7 text-slate-600">{{ post().description }}</p>
                        <div class="mt-5 border-t border-slate-200 pt-4 text-sm">
                            <p class="font-bold text-slate-800">{{ post().phone }}</p>
                            <p class="mt-1 font-bold text-indigo-600">{{ post().email }}</p>
                            <a routerLink="/posts/{{post().id}}" class="mt-4 inline-flex rounded-lg bg-slate-950 px-4 py-2 text-sm font-bold text-white">View post</a>
                        </div>  
                    </article>`,
  styles: ``,
})
export class PostItem {

    postBadgesService: PostsBadgeService = inject(PostsBadgeService);
    badgesService: BadgesService = inject(BadgesService);

    // imam sve badge-id,eve za taj post
    // Sada treba da odem do badges i vratim sve badgeve koje odgovaraj id-evima u badges nizu objekata
    // Ili mogu da ucitam sve badge-eve i ona izvucem samo one objekte kojima odgovara prosledjeni id iz badges

    // Sta sad radim?
    // Pravim servis kojim dohvatam sve podatke sa /badges endpoint-a
    
    badgesIds = signal<PostBadgesInterface[] | undefined>([]); // -> niz objekata
    badgesIdsArray: string[] = []; // -> sada imam niz id-eva, sada iz svih dobijenih badgeva izvlacim samo one ciji se id nalazi u badgesIdsArray


    badges = signal<BadgesInterface[]>([]);

   post = input.required<PostsInterface>();

   // U liniji ispod imam sve badgeve za specifican post
   postBadges = this.badges().filter(badge => {
    return this.badgesIdsArray.includes(badge.id);
   })

  ngOnInit() {
    this.postBadgesService.getPostBadges(this.post().id).then(data => {
      this.badgesIds.set(data);
    })

    this.badgesService.getAllBadges().then(data => {
      this.badges.set(data);
    })

    this.badgesIds()?.forEach((element: PostBadgesInterface) => {
      this.badgesIdsArray.push(element.id);
    });
  }

  
}
