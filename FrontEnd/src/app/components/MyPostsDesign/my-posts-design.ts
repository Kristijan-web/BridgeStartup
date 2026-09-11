import { Component, effect, inject, signal, untracked } from '@angular/core';
import { DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { toSignal } from '@angular/core/rxjs-interop';
import { PostsService } from '../../services/posts-service';
import { AuthService } from '../../services/auth-service';
import { errorMessage } from '../../services/api-service';
import { BadgeChoice, CreatePostInput, OwnedPost, PostApplicant } from '../../interfaces/posts-interface';

@Component({
  selector: 'app-my-posts-design', imports: [FormsModule, RouterLink, DatePipe],
  template: `<main class="mx-auto max-w-6xl px-6 py-10">
    <div class="flex flex-wrap items-center justify-between gap-4">
      <div><p class="eyebrow">Your workspace</p><h1>My posts</h1><p class="mt-2 text-slate-500">Share an idea and meet the people who want to help build it.</p></div>
      <button class="button" (click)="openCreate()">New post</button>
    </div>
    @if (success()) { <p class="success" role="status">{{ success() }}</p> }
    @if (creating()) {
      <section class="panel mt-6" aria-labelledby="create-post-title">
        <h2 id="create-post-title" class="text-xl font-black">Create a post</h2>
        @if (saveError()) { <p class="error" role="alert">{{ saveError() }}</p> }
        <form #form="ngForm" (ngSubmit)="publish()" class="form-grid">
          <fieldset [disabled]="saving()" class="grid gap-4 sm:grid-cols-2">
            <label class="sm:col-span-2">Title<input name="title" required minlength="3" maxlength="100" [(ngModel)]="draft.title"></label>
            <label class="sm:col-span-2">Description<textarea name="description" rows="5" required minlength="10" [(ngModel)]="draft.description"></textarea></label>
            <label>Contact email<input name="email" type="email" required email [(ngModel)]="draft.email"></label>
            <label>Phone<input name="phone" type="tel" required [(ngModel)]="draft.phone"></label>
            <fieldset class="sm:col-span-2"><legend class="mb-2 font-bold">Skills needed</legend>
              @if (badgesLoading()) { <p role="status">Loading skills…</p> }
              @else if (badgesError()) { <p class="error" role="alert">{{ badgesError() }} <button type="button" class="underline" (click)="loadBadges()">Retry</button></p> }
              @else {
                <div class="flex flex-wrap gap-3">
                  @for (badge of badges(); track badge.id) {
                    <label class="flex items-center gap-2 rounded-lg border border-slate-200 px-3 py-2">
                      <input type="checkbox" class="!w-auto" [checked]="draft.badges.includes(badge.id)" (change)="toggleBadge(badge.id, $event)">{{ badge.name }}
                    </label>
                  } @empty { <p class="help">No skills are available yet. Please try again later.</p> }
                </div><p class="help mt-2">Choose at least one skill.</p>
              }
            </fieldset>
          </fieldset>
          <div class="flex gap-3"><button class="button" [disabled]="form.invalid || saving() || badgesLoading() || !!badgesError() || !draft.badges.length">{{ saving() ? 'Publishing…' : 'Publish post' }}</button>
            <button type="button" class="button secondary" [disabled]="saving()" (click)="closeCreate()">Cancel</button></div>
        </form>
      </section>
    }
    <section class="panel mt-6" aria-labelledby="owned-posts-title">
      <div class="mb-4 flex items-center justify-between gap-3"><h2 id="owned-posts-title" class="text-xl font-black">Published posts</h2>
        <button class="button secondary" [disabled]="loading()" (click)="loadPosts()">Refresh posts</button></div>
      @if (loading()) { <p role="status">Loading your posts…</p> }
      @else if (error()) { <p class="error" role="alert">{{ error() }} <button class="underline" (click)="loadPosts()">Retry</button></p> }
      @else {
        <ul class="divide-y divide-slate-200">
          @for (post of posts(); track post.id) {
            <li class="flex flex-wrap items-center justify-between gap-4 py-4">
              <div><a [routerLink]="['/posts', post.id]" class="font-bold text-indigo-700 underline">{{ post.title }}</a>
                <p class="help">{{ post.applicationCount }} {{ post.applicationCount === 1 ? 'application' : 'applications' }}</p></div>
              <button class="button secondary" (click)="review(post)" [attr.aria-label]="'Review applicants for ' + post.title">Review applicants</button>
            </li>
          } @empty { <li class="py-4 text-slate-500">No posts yet. Create a post to share your idea.</li> }
        </ul>
      }
      <nav class="mt-4 flex items-center gap-4" aria-label="Your post pages">
        <button class="button secondary" [disabled]="loading() || page() === 1" (click)="loadPosts(page() - 1)">Previous</button>
        <span>Page {{ page() }}</span><button class="button secondary" [disabled]="loading() || !!error() || posts().length < 10" (click)="loadPosts(page() + 1)">Next</button>
      </nav>
    </section>
    @if (selected(); as post) {
      <section class="panel mt-6" aria-labelledby="applicants-title">
        <div class="flex flex-wrap items-center justify-between gap-3"><h2 id="applicants-title" class="text-xl font-black">Applicants for {{ post.title }}</h2>
          <button class="button secondary" (click)="review(post)" [disabled]="applicantsLoading()">Refresh applicants</button></div>
        @if (applicantsLoading()) { <p class="mt-4" role="status">Loading applicants…</p> }
        @else if (applicantsError()) { <p class="error" role="alert">{{ applicantsError() }}</p> }
        @else {
          <ul class="mt-4 divide-y divide-slate-200">
            @for (applicant of applicants(); track applicant.userId) {
              <li class="flex flex-wrap items-center justify-between gap-4 py-4">
                <div><p class="font-bold">{{ applicant.username }}</p><p class="help">Applied {{ applicant.createdAt | date:'medium' }}</p><p class="help">{{ applicant.fileName }}</p></div>
                <button class="button secondary" [disabled]="downloading() !== null" (click)="download(post.id, applicant)" [attr.aria-label]="'Download CV from ' + applicant.username">{{ downloading() === applicant.userId ? 'Downloading…' : 'Download CV' }}</button>
              </li>
            } @empty { <li class="py-4 text-slate-500">No one has applied yet.</li> }
          </ul>
        }
        @if (downloadError()) { <p class="error" role="alert">{{ downloadError() }}</p> }
      </section>
    }
  </main>`
})
export class MyPostsDesign {
  private service = inject(PostsService);
  private auth = inject(AuthService);
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private params = toSignal(this.route.queryParamMap);
  posts = signal<OwnedPost[]>([]); page = signal(1); loading = signal(false); error = signal('');
  creating = signal(false); saving = signal(false); saveError = signal(''); success = signal('');
  badges = signal<BadgeChoice[]>([]); badgesLoading = signal(false); badgesError = signal('');
  selected = signal<OwnedPost | null>(null); applicants = signal<PostApplicant[]>([]);
  applicantsLoading = signal(false); applicantsError = signal('');
  downloading = signal<number | null>(null); downloadError = signal('');
  private reviewVersion = 0;
  private postsVersion = 0;
  draft: CreatePostInput = this.emptyDraft();
  constructor() {
    effect(() => { if (this.params()?.get('create') === '1') untracked(() => { void this.openCreate(); }); });
  }
  ngOnInit() { void this.loadPosts(); }
  private emptyDraft(): CreatePostInput { return { title: '', description: '', email: this.auth.user()?.email ?? '', phone: '', badges: [] }; }
  async openCreate() { this.creating.set(true); if (!this.badges().length) await this.loadBadges(); }
  closeCreate() {
    this.creating.set(false);
    void this.router.navigate([], { relativeTo: this.route, queryParams: { create: null }, queryParamsHandling: 'merge', replaceUrl: true });
  }
  async loadBadges() {
    if (this.badgesLoading()) return;
    this.badgesLoading.set(true); this.badgesError.set('');
    try { this.badges.set(await this.service.getBadges()); }
    catch (error) { this.badgesError.set(errorMessage(error)); }
    finally { this.badgesLoading.set(false); }
  }
  toggleBadge(id: number, event: Event) {
    this.draft.badges = (event.target as HTMLInputElement).checked ? [...new Set([...this.draft.badges, id])] : this.draft.badges.filter(value => value !== id);
  }
  async publish() {
    if (this.saving()) return;
    this.saving.set(true); this.saveError.set(''); this.success.set('');
    try {
      await this.service.createPost({ ...this.draft, title: this.draft.title.trim(), description: this.draft.description.trim(), email: this.draft.email.trim(), phone: this.draft.phone.trim() });
      this.closeCreate(); this.draft = this.emptyDraft(); this.success.set('Your post has been published.');
      await this.loadPosts(1);
    } catch (error) { this.saveError.set(errorMessage(error)); }
    finally { this.saving.set(false); }
  }
  async loadPosts(page = this.page()) {
    const version = ++this.postsVersion;
    this.loading.set(true); this.error.set('');
    try {
      const posts = await this.service.getMyPosts(page);
      if (version === this.postsVersion) { this.posts.set(posts); this.page.set(page); }
    }
    catch (error) { if (version === this.postsVersion) this.error.set(errorMessage(error)); }
    finally { if (version === this.postsVersion) this.loading.set(false); }
  }
  async review(post: OwnedPost) {
    const version = ++this.reviewVersion;
    this.selected.set(post); this.applicants.set([]); this.applicantsLoading.set(true); this.applicantsError.set(''); this.downloadError.set('');
    try { const applicants = await this.service.getApplicants(post.id); if (version === this.reviewVersion) this.applicants.set(applicants); }
    catch (error) { if (version === this.reviewVersion) this.applicantsError.set(errorMessage(error)); }
    finally { if (version === this.reviewVersion) this.applicantsLoading.set(false); }
  }
  async download(postId: number, applicant: PostApplicant) {
    if (this.downloading() !== null) return;
    this.downloading.set(applicant.userId); this.downloadError.set('');
    try {
      const blob = await this.service.downloadCv(postId, applicant.userId);
      const url = URL.createObjectURL(blob);
      const link = document.createElement('a'); link.href = url; link.download = applicant.fileName;
      document.body.appendChild(link); link.click(); link.remove();
      setTimeout(() => URL.revokeObjectURL(url), 1000);
    } catch (error) { this.downloadError.set(errorMessage(error)); }
    finally { this.downloading.set(null); }
  }
}
