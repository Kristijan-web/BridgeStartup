import { Component, inject, signal } from '@angular/core';
import { PostsInterface } from '../../interfaces/posts-interface';
import { PostsService } from '../../services/posts-service';
import { PostItem } from './post-item';

@Component({
  selector: 'app-home-design',
  imports: [PostItem],
  // sada treba template da se popouni
  template: ` <section class="mx-auto max-w-6xl px-4 py-12 sm:px-6 lg:px-8">
            <div class="grid gap-8 lg:grid-cols-[0.95fr_1.05fr] lg:items-end">
                <div>
                    <p class="text-sm font-black uppercase tracking-[0.22em] text-cyan-700">Startup opportunities</p>
                    <h1 class="mt-4 max-w-2xl text-4xl font-black tracking-tight text-slate-950 sm:text-5xl">Bridge the gap between founders and builders.</h1>
                    <p class="mt-5 max-w-2xl text-lg leading-8 text-slate-600">BridgeStartup is a static board for startup posts. Founders can present an idea, stack, contact details, and project stage, while developers can quickly decide whether it fits them.</p>
                </div>
                <div class="grid gap-3 sm:grid-cols-3">
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-indigo-600">6</span>
                        <span class="text-sm font-semibold text-slate-500">open posts</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-emerald-600">24</span>
                        <span class="text-sm font-semibold text-slate-500">stack badges</span>
                    </div>
                    <div class="rounded-lg border border-slate-200 bg-white p-5 shadow-sm">
                        <span class="block text-2xl font-black text-amber-500">0</span>
                        <span class="text-sm font-semibold text-slate-500">dynamic parts</span>
                    </div>
                </div>
            </div>
        </section>

        <section class="border-y border-slate-200 bg-white">
            <div class="mx-auto max-w-6xl px-4 py-10 sm:px-6 lg:px-8">
                <div class="mb-7 flex flex-col gap-3 sm:flex-row sm:items-end sm:justify-between">
                    <div>
                        <p class="text-sm font-black uppercase tracking-[0.22em] text-indigo-600">Posts</p>
                        <h2 class="mt-2 text-3xl font-black tracking-tight">Fresh startup ideas</h2>
                    </div>
                   
                </div>

                <div class="grid gap-5 lg:grid-cols-3">
              // OVDE SE UBACUJE ARTIKLE
              // Za svaki element u array-u poziva se jedan post-item
              // Kako se to radi?
              // Prvo mi treba niz post-ova sa back-a
              // Ovo znaci da prvo moram da uzmem podatke iz DI container-a

              // sada treba da se koristi strukturna direktiva 
              @for (post of this.posts(); track post.id) {

                <post-item></post-item>

              }


                </div>
            </div>
        </section>`,
  styles: ``,
})
export class HomeDesign {

  postsService: PostsService = inject(PostsService) 

  posts= signal<PostsInterface[]>([]);

  
  ngOnInit() {
 
    this.postsService.getAllPosts().then(data => {
      this.posts.set(data);
    });


    
  }

}
