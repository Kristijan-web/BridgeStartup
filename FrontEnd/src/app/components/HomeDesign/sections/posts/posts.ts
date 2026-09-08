import { Component, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { PostsService } from '../../../../services/posts-service';
import { PostsInterface } from '../../../../interfaces/posts-interface';
import { errorMessage } from '../../../../services/api-service';
import { PostItem } from './postsItem/posts-item';
@Component({
  selector: 'app-posts', imports: [PostItem, FormsModule],
  template: `<section class="border-y border-slate-200 bg-white">
    <div class="mx-auto max-w-6xl px-6 py-10">
      <p class="eyebrow">Posts</p><h2 class="mt-2 text-3xl font-black">Fresh startup ideas</h2>
      <form (ngSubmit)="load(1)" class="my-6 flex flex-wrap gap-3">
        <label class="grow"><span class="sr-only">Search posts by title</span><input name="title" [(ngModel)]="title" placeholder="Search posts by title"></label>
        <label><span class="sr-only">Sort posts</span>
          <select name="sortOrder" [(ngModel)]="sortOrder" (ngModelChange)="load(1)" [disabled]="loading()" aria-label="Sort posts">
            <option value="asc">Title: A to Z</option><option value="desc">Title: Z to A</option>
          </select>
        </label>
        <button class="button" [disabled]="loading()">Search</button>
      </form>
      @if (loading()) { <p role="status">Loading posts…</p> }
      @else if (error()) { <div class="error" role="alert">{{ error() }} <button class="underline" (click)="load()">Retry</button></div> }
      @else {
        <div class="grid gap-5 md:grid-cols-2 lg:grid-cols-3">
          @for (post of posts(); track post.id) { <app-post-item [post]="post" /> }
          @empty { <p class="text-slate-600">No posts found. Try another search.</p> }
        </div>
      }
      <nav class="mt-6 flex items-center gap-4" aria-label="Post pages">
        <button class="button secondary" (click)="load(page() - 1)" [disabled]="loading() || page() === 1">Previous</button>
        <span aria-live="polite">Page {{ page() }}</span>
        <button class="button secondary" (click)="load(page() + 1)" [disabled]="loading() || !!error() || posts().length < 5">Next</button>
      </nav>
    </div>
  </section>`
})
export class Posts {
  private service = inject(PostsService);
  posts = signal<PostsInterface[]>([]); loading = signal(false); error = signal('');
  title = '';
  sortOrder: 'asc' | 'desc' = 'asc';
  page = signal(1);
  ngOnInit() { void this.load(); }
  async load(page = this.page()) {
    if (this.loading()) return;
    this.loading.set(true); this.error.set('');
    try {
      const posts = await this.service.getAllPosts(this.title, this.sortOrder, page);
      this.posts.set(posts); this.page.set(page);
    }
    catch (error) { this.error.set(errorMessage(error)); }
    finally { this.loading.set(false); }
  }
}
