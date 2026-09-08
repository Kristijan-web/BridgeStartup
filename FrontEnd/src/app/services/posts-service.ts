import { inject, Injectable } from '@angular/core';
import { ApiService } from './api-service';
import { PostsInterface } from '../interfaces/posts-interface';

@Injectable({ providedIn: 'root' })
export class PostsService {
  private api = inject(ApiService);
  getAllPosts(title = '', sortOrder: 'asc' | 'desc' = 'asc'): Promise<PostsInterface[]> {
    const query = new URLSearchParams();
    if (title.trim()) query.set('Title', title.trim());
    query.set('SortBy', 'title');
    query.set('SortOrder', sortOrder);
    return this.api.request('/Posts?' + query);
  }
  getPost(id: string | number): Promise<PostsInterface> {
    return this.api.request('/Posts/' + encodeURIComponent(id));
  }
  applyToPost(postId: number, file: File): Promise<void> {
    const form = new FormData();
    form.set('PostId', String(postId));
    form.set('userFile', file, file.name);
    return this.api.request('/Posts', { method: 'POST', body: form });
  }
}
