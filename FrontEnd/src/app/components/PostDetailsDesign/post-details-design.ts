import { Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PostsService } from '../../services/posts-service';
import { PostsInterface } from '../../interfaces/posts-interface';
import { DatePipe } from '@angular/common';
import { PostsBadgeService } from '../../services/posts-badge-service';
import { BadgesService } from '../../services/badges-service';
import { PostBadgesInterface } from '../../interfaces/post-badges-interface';
import { BadgesInterface } from '../../interfaces/badges-interface';
import { BadgeItem } from '../../common/badge-item';
import { UserService } from '../../services/user-service';
import { UserInterface } from '../../interfaces/user-interface';

@Component({
  selector: 'app-post-details-design',
  imports: [DatePipe, BadgeItem, RouterLink],
  template: `  <main class="flex-1">
      <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
        <a routerLink="" class="inline-flex items-center cursor-pointer rounded-lg bg-slate-100 px-4 py-2 text-sm font-bold text-slate-700 hover:bg-slate-200">
          Back to posts
        </a>

        <div class="mt-8 grid gap-8 lg:grid-cols-[1fr_360px] lg:items-start">
          <article class="rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm sm:p-8">
            <div class="mb-5 flex flex-wrap gap-2">
              @for(badge of postBadges(); track badge.id) {
                  <app-badge-item [badgeName]="badge.name"></app-badge-item>
              }

            </div>

            <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Post detail</p>
            <h1 class="mt-4 max-w-3xl text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">
              {{ post()?.title }}
            </h1>

        
            <div class="mt-8 grid gap-4 border-y border-slate-200 py-6 sm:grid-cols-3">
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Created</span>
                <span class="mt-2 block font-bold text-slate-950">{{ post()?.createdAt | date:'dd.MM.yyyy.' }}</span>
              </div>
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Founder</span>
                <span class="mt-2 block font-bold text-slate-950">{{ userPost()?.username }}</span>
              </div>
              <div>
                <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Status</span>
                <span class="mt-2 block font-bold text-emerald-600">Available</span>
              </div>
            </div>

            <div class="mt-8">
              <h2 class="text-2xl font-black tracking-tight">Project overview</h2>
              <div class="mt-4 space-y-4 leading-8 text-slate-600">
                <p>
                  {{ post()?.description }}
                </p>
          
              </div>
            </div>

          </article>

          <aside class="space-y-5">
            <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
              <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Contact</p>
              <div class="mt-5 space-y-4">
                <div>
                  <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Phone</span>
                  <p class="mt-2 font-bold text-slate-950">{{ post()?.phone }}</p>
                </div>
                <div>
                  <span class="block text-xs font-black uppercase tracking-[0.16em] text-slate-500">Email</span>
                  <p class="mt-2 break-words font-bold text-indigo-600">{{post()?.email}}</p>
                </div>
              </div>
              <a href="mailto:Sylvester_Smitham@yahoo.com" class="mt-6 inline-flex w-full justify-center rounded-lg bg-slate-950 px-5 py-3 text-sm font-black text-white shadow-sm">
                Send message
              </a>
            </div>

            <div class="rounded-lg border border-slate-200 bg-[#f8fbff] p-6 shadow-sm">
              <p class="text-sm font-black uppercase tracking-[0.22em] text-cyan-700">Stack</p>
              <div class="mt-5 flex flex-wrap gap-2">
              @for(badge of postBadges(); track badge.id) {
                  <app-badge-item [badgeName]="badge.name"></app-badge-item>
              }
              </div>
            </div>

            <div class="rounded-lg border border-slate-200 bg-white p-6 shadow-sm">
              <span class="block text-2xl font-black text-indigo-600">1</span>
              <span class="text-sm font-semibold text-slate-500">matching stack badge</span>
            </div>
          </aside>
        </div>
      </section>
    </main>
 `,
  styles: ``,
})
export class PostDetailsDesign {

  // treba da dinamicki ispisem badge-eve
  
  // Sta mi je potrebno da bi prikazao badgeve?
  // - computed polje koja racuna kada se promeni signal
  // Fale mi badges i post bagdes

  private postsService = inject(PostsService);
  private route = inject(ActivatedRoute);
  private postBadgesService = inject(PostsBadgeService);
  private badgesService = inject(BadgesService);
  private userService = inject(UserService);
  // Sta sad treba da uradim?
  id!: string;

  post = signal<PostsInterface | undefined>(undefined);
  // treba mi signal za postBadgesIds
  postBadgesIds = signal<PostBadgesInterface[] | undefined>([])
  // treba mi signal za badges
  badges = signal<BadgesInterface[]>([])

  userPost = signal<UserInterface | undefined>(undefined);

  // Treba da dohvatim usera iz post-a

  // sada mi treba computed da se filteruju badgeve

  // kako ide sintaksa za computed??
  // To je neka funkcija
  postBadges = computed(() => {

    const badgeIds = this.postBadgesIds()?.map(postBadge => Number(postBadge.badgeId));

    const filteredBadges = this.badges().filter(badge =>{   
         
    return  badgeIds?.includes(Number(badge.id))
    }
    
    );
  
    return filteredBadges;
  })

  // treba mi computed koji ce se trigerovati kada se promeni vrednost polja this.post()

  postUser = computed(() => {
    // samo posa
  })
// Sta sad treba da se uradi?
// - Treba da se pozove userServis i dohvati user za prosledjeni post-id
// 



 async ngOnInit() {
    this.id = this.route.snapshot.paramMap.get('id')!;

    await this.postsService.getPost(this.id).then(data => {
      this.post.set(data);
      // mora drugi asinhroni zahtev ovde da se napravi
    })

    this.postBadgesService
      .getPostBadges(this.id)
      .then(data => {
        this.postBadgesIds.set(data);
      });

    this.badgesService
      .getAllBadges()
      .then(data => {
        this.badges.set(data);
      });


    this.userService.getUser(this.post()?.userId).then(data => {

      console.log('ALOOO')
      console.log(this.post()?.userId);

      console.log(data);

        this.userPost.set(data);

    })

  }

}
