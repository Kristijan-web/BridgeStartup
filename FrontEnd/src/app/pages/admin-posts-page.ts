import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AdminService } from '../services/user-service';
import { errorMessage } from '../services/api-service';
import { AdminUser } from '../interfaces/user-interface';
import { AdminPost, PostInput } from '../interfaces/posts-interface';
@Component({
  selector: 'app-admin-posts', imports: [FormsModule, RouterLink],
  template: `
    <div class="flex flex-wrap items-center justify-between gap-4">
      <div><p class="eyebrow">Administration</p><h1>Posts</h1><p class="mt-2 text-slate-500">Publish and manage startup ideas.</p></div>
      <button class="button" (click)="create()" [disabled]="loading() || busy() || !users().length">Add post</button>
    </div>
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (success()) { <p class="success" role="status">{{ success() }}</p> }
    @if (editing()) {
      <section class="panel mt-6" aria-labelledby="post-form-title">
        <h2 id="post-form-title" class="text-xl font-black">{{ editId === null ? 'Create post' : 'Edit post' }}</h2>
        <form #form="ngForm" (ngSubmit)="save()" class="form-grid">
          <fieldset [disabled]="busy()" class="grid gap-4 sm:grid-cols-2">
            <label class="sm:col-span-2">Title<input name="title" required maxlength="200" [(ngModel)]="draft.title"></label>
            <label class="sm:col-span-2">Description<textarea name="description" rows="6" required maxlength="20000" [(ngModel)]="draft.description"></textarea></label>
            <label>Founder<select name="userId" required [(ngModel)]="draft.userId">
              @for (user of users(); track user.id) { <option [ngValue]="user.id">{{ user.username }} ({{ user.email }})</option> }
            </select></label>
            <label>Contact email (optional)<input name="email" type="email" email [(ngModel)]="draft.email"></label>
            <label>Phone (optional)<input name="phone" type="tel" maxlength="50" [(ngModel)]="draft.phone"></label>
            <label>Stack badges<input name="badges" [(ngModel)]="badges" placeholder="C#, TypeScript, Design"><span class="help">Separate badge names with commas. Leave blank to remove all badges.</span></label>
          </fieldset>
          <div class="flex gap-3"><button class="button" [disabled]="form.invalid || busy()">{{ busy() ? 'Saving…' : 'Save post' }}</button>
            <button type="button" class="button secondary" (click)="editing.set(false)" [disabled]="busy()">Cancel</button></div>
        </form>
      </section>
    }
    @if (deleting(); as post) {
      <section class="panel mt-6 border-red-200" role="alert" aria-labelledby="delete-post-title">
        <h2 id="delete-post-title" class="text-xl font-black">Delete “{{ post.title }}”?</h2>
        <p class="my-4">The post will be permanently deleted.</p>
        <div class="flex gap-3"><button class="button danger" (click)="confirmDelete()" [disabled]="busy()">{{ busy() ? 'Deleting…' : 'Delete post' }}</button>
          <button class="button secondary" (click)="deleting.set(null)" [disabled]="busy()">Cancel</button></div>
      </section>
    }
    <section class="panel mt-6">
      <div class="mb-5 flex flex-wrap gap-3">
        <label class="grow"><span class="sr-only">Search this page</span><input placeholder="Search title or founder on this page" [ngModel]="search()" (ngModelChange)="search.set($event)"></label>
        <label><span class="sr-only">Sort posts</span><select name="sortOrder" [(ngModel)]="sortOrder" (ngModelChange)="load(1)" [disabled]="loading() || busy()" aria-label="Sort posts">
          <option value="asc">Title: A to Z</option><option value="desc">Title: Z to A</option>
        </select></label>
        <button class="button secondary" (click)="load()" [disabled]="loading() || busy()">Refresh</button>
      </div>
      @if (loading()) { <p role="status">Loading posts…</p> }
      @else {
        <p class="help mb-3">{{ filtered().length }} posts on this page</p>
        <div class="overflow-x-auto"><table class="w-full">
          <thead><tr><th>Title</th><th>Founder</th><th>Badges</th><th>Actions</th></tr></thead>
          <tbody>@for (post of filtered(); track post.id) {
            <tr><td class="font-bold"><a [routerLink]="['/posts', post.id]" class="text-indigo-700 underline">{{ post.title }}</a></td>
              <td>{{ post.user.username }}</td><td>{{ post.badges.join(', ') || '—' }}</td>
              <td><div class="flex gap-2">
                <button class="button secondary" (click)="edit(post)" [disabled]="busy()" [attr.aria-label]="'Edit ' + post.title">Edit</button>
                <button class="button secondary" (click)="askDelete(post)" [disabled]="busy()" [attr.aria-label]="'Delete ' + post.title">Delete</button>
              </div></td></tr>
          } @empty { <tr><td colspan="4">No posts found.</td></tr> }</tbody>
        </table></div>
      }
      <nav class="mt-5 flex items-center gap-4" aria-label="Admin post pages">
        <button class="button secondary" (click)="load(page() - 1)" [disabled]="loading() || busy() || page() === 1">Previous</button>
        <span aria-live="polite">Page {{ page() }}</span>
        <button class="button secondary" (click)="load(page() + 1)" [disabled]="loading() || busy() || !!error() || posts().length < 5">Next</button>
      </nav>
    </section>`
})
export class AdminPostsPage {
  private service = inject(AdminService);
  posts = signal<AdminPost[]>([]); users = signal<AdminUser[]>([]);
  loading = signal(false); busy = signal(false); error = signal(''); success = signal('');
  search = signal(''); editing = signal(false); deleting = signal<AdminPost | null>(null);
  sortOrder: 'asc' | 'desc' = 'asc';
  page = signal(1);
  editId: number | null = null; draft: PostInput = this.emptyDraft(); badges = '';
  filtered = computed(() => {
    const term = this.search().trim().toLowerCase();
    return this.posts().filter(post => (post.title + ' ' + post.user.username).toLowerCase().includes(term));
  });
  ngOnInit() { void this.load(); }
  private emptyDraft(): PostInput { return { title: '', description: '', email: '', phone: '', userId: 0, badges: [] }; }
  create() {
    this.editId = null; this.draft = this.emptyDraft(); this.draft.userId = this.users()[0].id; this.badges = '';
    this.error.set(''); this.success.set(''); this.deleting.set(null); this.editing.set(true);
  }
  edit(post: AdminPost) {
    this.editId = post.id;
    this.draft = { title: post.title, description: post.description, email: post.email ?? '', phone: post.phone ?? '', userId: post.userId, badges: [...post.badges] };
    this.badges = post.badges.join(', ');
    this.error.set(''); this.success.set(''); this.deleting.set(null); this.editing.set(true);
  }
  askDelete(post: AdminPost) { this.editing.set(false); this.error.set(''); this.success.set(''); this.deleting.set(post); }
  async load(page = this.page()) {
    if (this.loading()) return;
    this.loading.set(true); this.error.set('');
    try {
      const [posts, users] = await Promise.all([this.service.getPosts(this.sortOrder, page), this.service.getUsers()]);
      this.posts.set(posts); this.users.set(users); this.page.set(page);
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.loading.set(false); }
  }
  async save() {
    if (this.busy()) return;
    this.busy.set(true); this.error.set(''); this.success.set('');
    try {
      const input: PostInput = { ...this.draft, title: this.draft.title.trim(), description: this.draft.description.trim(),
        email: this.draft.email?.trim() || null, phone: this.draft.phone?.trim() || null,
        badges: [...new Set(this.badges.split(',').map(b => b.trim()).filter(Boolean))] };
      if (this.editId === null) await this.service.createPost(input);
      else await this.service.updatePost(this.editId, input);
      this.editing.set(false); this.success.set('Post saved.'); await this.load(1);
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
  async confirmDelete() {
    const post = this.deleting();
    if (!post || this.busy()) return;
    this.busy.set(true); this.error.set(''); this.success.set('');
    try {
      await this.service.deletePost(post.id);
      this.deleting.set(null); this.success.set('Post deleted.'); await this.load(this.posts().length === 1 && this.page() > 1 ? this.page() - 1 : this.page());
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
}
