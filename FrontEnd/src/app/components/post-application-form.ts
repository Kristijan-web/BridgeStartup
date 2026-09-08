import { Component, effect, inject, input, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../services/auth-service';
import { ApiError, errorMessage } from '../services/api-service';
import { PostsService } from '../services/posts-service';

export const MAX_CV_BYTES = 5 * 1024 * 1024;
export function validateCv(file: File): string {
  if (!file.size || file.size > MAX_CV_BYTES) return 'Choose a non-empty CV up to 5 MB.';
  if (!/\.(pdf|doc|docx)$/i.test(file.name)) return 'Upload a PDF, DOC, or DOCX file.';
  return '';
}

@Component({
  selector: 'app-post-application-form', imports: [FormsModule, RouterLink],
  template: `
    <p class="eyebrow">Join this startup</p>
    <h2 class="mt-3 text-xl font-black">Apply to this post</h2>
    @if (!auth.user()) {
      <p class="my-5 text-slate-600">Sign in to submit your CV to this startup.</p>
      <a routerLink="/login" [queryParams]="{ returnUrl: '/posts/' + postId() }" class="button">Sign in to apply</a>
    } @else if (submitted()) {
      <p class="success" role="status">{{ submitted() }}</p>
    } @else {
      <p class="mt-3 text-sm text-slate-600">Introduce yourself by uploading your CV.</p>
      <form (ngSubmit)="submit()" class="form-grid">
        <label>CV / resume
          <input type="file" name="userFile" accept=".pdf,.doc,.docx" (change)="selectFile($event)" [disabled]="busy()" aria-describedby="cv-help">
        </label>
        <p id="cv-help" class="help">PDF, DOC, or DOCX. Maximum 5 MB.</p>
        @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
        <button class="button" [disabled]="!file() || busy()">{{ busy() ? 'Submitting application…' : 'Submit application' }}</button>
      </form>
    }
  `
})
export class PostApplicationForm {
  readonly auth = inject(AuthService);
  private posts = inject(PostsService);
  postId = input.required<number>();
  file = signal<File | null>(null);
  busy = signal(false); error = signal(''); submitted = signal('');
  private generation = 0;

  constructor() {
    effect(() => {
      this.postId(); this.auth.user();
      this.generation++;
      this.file.set(null); this.error.set(''); this.submitted.set(''); this.busy.set(false);
    });
  }
  selectFile(event: Event) {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0] ?? null;
    const error = file ? validateCv(file) : '';
    this.error.set(error); this.file.set(error ? null : file);
    if (error) input.value = '';
  }
  async submit() {
    const file = this.file();
    if (!file || this.busy() || !this.auth.getToken()) return;
    const generation = this.generation;
    this.busy.set(true); this.error.set('');
    try {
      await this.posts.applyToPost(this.postId(), file);
      if (generation === this.generation) { this.submitted.set('Your application has been submitted.'); this.file.set(null); }
    } catch (error) {
      if (generation === this.generation) {
        if (error instanceof ApiError && error.status === 409) this.submitted.set('You have already applied to this post.');
        else this.error.set(errorMessage(error));
      }
    } finally { if (generation === this.generation) this.busy.set(false); }
  }
}
