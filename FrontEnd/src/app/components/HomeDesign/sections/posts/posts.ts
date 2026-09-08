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
      <form (ngSubmit)="load()" class="my-6 flex flex-wrap gap-3">
        <label class="grow"><span class="sr-only">Search posts by title</span><input name="title" [(ngModel)]="title" placeholder="Search posts by title"></label>
        <label><span class="sr-only">Sort posts</span>
          <select name="sortOrder" [(ngModel)]="sortOrder" (ngModelChange)="load()" [disabled]="loading()" aria-label="Sort posts">
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
    </div>
  </section>`
})
export class Posts {
  private service = inject(PostsService);
  posts = signal<PostsInterface[]>([]); loading = signal(false); error = signal('');
  title = '';
  sortOrder: 'asc' | 'desc' = 'asc';
  ngOnInit() { void this.load(); }
  async load() {
    if (this.loading()) return;
    this.loading.set(true); this.error.set('');
    try { this.posts.set(await this.service.getAllPosts(this.title, this.sortOrder)); }
    catch (error) { this.error.set(errorMessage(error)); }
    finally { this.loading.set(false); }
  }
}
