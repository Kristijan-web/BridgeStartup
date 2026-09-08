import { inject, Injectable } from '@angular/core';
import { ApiService } from './api-service';
import { AdminUser, Role, UserInput } from '../interfaces/user-interface';
import { AdminPost, PostInput } from '../interfaces/posts-interface';
import { PostsService } from './posts-service';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = inject(ApiService);
  private posts = inject(PostsService);
  getUsers() { return this.api.request<AdminUser[]>('/Users'); }
  getRoles() { return this.api.request<Role[]>('/admin/roles'); }
  createUser(user: UserInput) { return this.api.request<AdminUser>('/Users', { method: 'POST', body: JSON.stringify(user) }); }
  updateUser(id: number, user: UserInput) { return this.api.request<void>('/Users/' + id, { method: 'PUT', body: JSON.stringify(user) }); }
  deleteUser(id: number) { return this.api.request<void>('/Users/' + id, { method: 'DELETE' }); }
  getPosts(sortOrder: 'asc' | 'desc' = 'asc') { return this.posts.getAllPosts('', sortOrder); }
  createPost(post: PostInput) { return this.api.request<AdminPost>('/Posts', { method: 'POST', body: JSON.stringify(post) }); }
  updatePost(id: number, post: PostInput) { return this.api.request<void>('/Posts/' + id, { method: 'PATCH', body: JSON.stringify(post) }); }
  deletePost(id: number) { return this.api.request<void>('/Posts/' + id, { method: 'DELETE' }); }
}
