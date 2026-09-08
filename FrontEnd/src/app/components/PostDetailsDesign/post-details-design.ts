import { Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { PostsService } from '../../services/posts-service';
import { PostsInterface } from '../../interfaces/posts-interface';
import { errorMessage } from '../../services/api-service';
import { BadgeItem } from '../../common/badge-item';
import { PostApplicationForm } from '../post-application-form';
@Component({
  selector: 'app-post-details-design', imports: [BadgeItem, RouterLink, PostApplicationForm],
  template: `<section class="mx-auto max-w-6xl px-6 py-12">
    <a routerLink="/" class="button secondary">Back to posts</a>
    @if (loading()) { <p class="mt-8" role="status">Loading post…</p> }
    @else if (error()) { <p class="error mt-8" role="alert">{{ error() }} <button class="underline" (click)="load(id)">Retry</button></p> }
    @else if (post(); as item) {
      <div class="mt-8 grid gap-8 lg:grid-cols-[1fr_320px]">
        <article class="panel">
          <div class="mb-5 flex flex-wrap gap-2">
            @for (badge of item.badges; track badge) { <app-badge-item [badgeName]="badge" /> }
          </div>
          <p class="eyebrow">Startup idea</p><h1>{{ item.title }}</h1>
          <p class="my-6 border-y border-slate-200 py-5 font-bold">Founder: {{ item.user.username }}</p>
          <h2 class="text-2xl font-black">Project overview</h2>
          <p class="mt-4 whitespace-pre-wrap leading-8 text-slate-600">{{ item.description }}</p>
        </article>
        <aside class="panel h-fit">
          <app-post-application-form [postId]="item.id" />
          <p class="mt-6 text-sm text-slate-500">{{ item.badges.length }} stack badges</p>
        </aside>
      </div>
    }
  </section>`
})
export class PostDetailsDesign {
  private service = inject(PostsService);
  private route = inject(ActivatedRoute);
  post = signal<PostsInterface | null>(null); loading = signal(true); error = signal('');
  id = ''; private requestId = 0;
  constructor() {
    this.route.paramMap.pipe(takeUntilDestroyed()).subscribe(params => {
      this.id = params.get('id') ?? '';
      void this.load(this.id);
    });
  }
  async load(id: string) {
    const request = ++this.requestId;
    this.loading.set(true); this.error.set(''); this.post.set(null);
    try {
      if (!/^[1-9][0-9]*$/.test(id)) throw new Error('This post could not be found.');
      const post = await this.service.getPost(id);
      if (request === this.requestId) this.post.set(post);
    } catch (error) { if (request === this.requestId) this.error.set(errorMessage(error)); }
    finally { if (request === this.requestId) this.loading.set(false); }
  }
}
