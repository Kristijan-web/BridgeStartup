import { inject, Injectable } from '@angular/core';
import { ApiService } from './api-service';
import { BadgeChoice, CreatePostInput, OwnedPost, PostApplicant, PostsInterface } from '../interfaces/posts-interface';

@Injectable({ providedIn: 'root' })
export class PostsService {
  private api = inject(ApiService);
  getBadges(): Promise<BadgeChoice[]> { return this.api.request('/Badges'); }
  createPost(post: CreatePostInput): Promise<void> {
    return this.api.request('/Posts', { method: 'POST', body: JSON.stringify(post) });
  }
  getMyPosts(page = 1): Promise<OwnedPost[]> { return this.api.request('/Posts/mine?Page=' + page); }
  getApplicants(postId: number): Promise<PostApplicant[]> { return this.api.request('/Posts/' + postId + '/applications'); }
  downloadCv(postId: number, userId: number): Promise<Blob> {
    return this.api.download('/PostApplications/' + postId + '/' + userId + '/file');
  }
  getAllPosts(title = '', sortOrder: 'asc' | 'desc' = 'asc', page = 1): Promise<PostsInterface[]> {
    const query = new URLSearchParams();
    if (title.trim()) query.set('Title', title.trim());
    query.set('SortBy', 'title');
    query.set('SortOrder', sortOrder);
    query.set('Page', String(page));
    return this.api.request('/Posts?' + query);
  }
  getPost(id: string | number): Promise<PostsInterface> {
    return this.api.request('/Posts/' + encodeURIComponent(id));
  }
  applyToPost(postId: number, file: File): Promise<void> {
    const form = new FormData();
    form.set('PostId', String(postId));
    form.set('userFile', file, file.name);
    return this.api.request('/Posts/apply', { method: 'POST', body: form });
  }
}
