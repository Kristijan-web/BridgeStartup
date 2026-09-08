import { inject, Injectable } from '@angular/core';
import { ApiService } from './api-service';
import { AdminUser, Role, UserInput } from '../interfaces/user-interface';
import { AdminPost, PostInput } from '../interfaces/posts-interface';
import { PostsService } from './posts-service';

@Injectable({ providedIn: 'root' })
export class AdminService {
  private api = inject(ApiService);
  private posts = inject(PostsService);
  // Resource user responses currently omit roleId/isActive and expose password hashes.
  // The existing admin DTO supplies the editable fields without those hashes.
  getUsers() { return this.api.request<AdminUser[]>('/admin/users'); }
  getRoles() { return this.api.request<Role[]>('/admin/roles'); }
  createUser(user: UserInput) { return this.api.request<AdminUser>('/admin/users', { method: 'POST', body: JSON.stringify(user) }); }
  async updateUser(id: number, user: UserInput) {
    const current = await this.api.request<AdminUser>('/admin/users/' + id);
    // Profile updates use the existing update-user command. Role/activation edits
    // still need the admin action because UpdateUserDTO has no such fields.
    if (current.roleId !== user.roleId || current.isActive !== user.isActive) {
      return this.api.request<void>('/admin/users/' + id, { method: 'PUT', body: JSON.stringify(user) });
    }
    return this.api.request<void>('/Users/' + id, { method: 'PATCH', body: JSON.stringify({
      username: user.username, email: user.email, password: user.password || undefined
    }) });
  }
  deleteUser(id: number) { return this.api.request<void>('/Users/' + id, { method: 'DELETE' }); }
  async getPosts(sortOrder: 'asc' | 'desc' = 'asc', page = 1): Promise<AdminPost[]> {
    // Use the resource query's sorting/pagination and the admin DTO's founder IDs.
    const [posts, details] = await Promise.all([
      this.posts.getAllPosts('', sortOrder, page), this.api.request<AdminPost[]>('/admin/posts')
    ]);
    const byId = new Map(details.map(post => [post.id, post]));
    return posts.map(post => {
      const userId = post.userId ?? byId.get(post.id)?.userId;
      if (userId === undefined) throw new Error('A post changed while loading. Refresh and try again.');
      return { ...post, userId };
    });
  }
  // POST /Posts needs badge IDs, but there is no catalog route to resolve names.
  createPost(post: PostInput) { return this.api.request<AdminPost>('/admin/posts', { method: 'POST', body: JSON.stringify(post) }); }
  // This action supports founder changes, new badges, and clearing optional fields.
  // The existing resource PATCH does not support those full-editor operations.
  updatePost(id: number, post: PostInput) { return this.api.request<void>('/admin/posts/' + id, { method: 'PUT', body: JSON.stringify(post) }); }
  deletePost(id: number) { return this.api.request<void>('/Posts/' + id, { method: 'DELETE' }); }
}
