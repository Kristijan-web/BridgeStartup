import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../services/user-service';
import { AuthService } from '../services/auth-service';
import { errorMessage } from '../services/api-service';
import { AdminUser, Role, UserInput } from '../interfaces/user-interface';
@Component({
  selector: 'app-admin-users', imports: [FormsModule],
  template: `
    <div class="flex flex-wrap items-center justify-between gap-4">
      <div><p class="eyebrow">Administration</p><h1>Users</h1><p class="mt-2 text-slate-500">Manage accounts, roles, and access.</p></div>
      <button class="button" (click)="create()" [disabled]="loading() || busy() || !roles().length">Add user</button>
    </div>
    @if (error()) { <p class="error" role="alert">{{ error() }}</p> }
    @if (success()) { <p class="success" role="status">{{ success() }}</p> }
    @if (editing()) {
      <section class="panel mt-6" aria-labelledby="user-form-title">
        <h2 id="user-form-title" class="text-xl font-black">{{ editId === null ? 'Create user' : 'Edit user' }}</h2>
        <form #form="ngForm" (ngSubmit)="save()" class="form-grid">
          <fieldset [disabled]="busy()" class="grid gap-4 sm:grid-cols-2">
            <label>Username<input name="username" required minlength="3" pattern="[^0-9].{2,}" [(ngModel)]="draft.username" autocomplete="off"></label>
            <label>Email<input name="email" type="email" email required [(ngModel)]="draft.email" autocomplete="off"></label>
            <label>Password {{ editId === null ? '' : '(leave blank to keep)' }}
              <input name="password" type="password" [required]="editId === null" minlength="8" pattern="(?=.*[A-Z])(?=.*[0-9]).{8,}" [(ngModel)]="draft.password" autocomplete="new-password">
              <span class="help">At least 8 characters, one uppercase letter, and one number.</span>
            </label>
            <label>Role<select name="roleId" required [(ngModel)]="draft.roleId" [disabled]="isSelf()">
              @for (role of roles(); track role.id) { <option [ngValue]="role.id">{{ role.name }}</option> }
            </select></label>
            <label class="flex items-center gap-2"><input name="isActive" type="checkbox" [(ngModel)]="draft.isActive" [disabled]="isSelf()">Account active</label>
          </fieldset>
          @if (isSelf()) { <p class="help">You cannot deactivate or change the role of your own admin account. Saving your account signs you out so you can sign in with the updated details.</p> }
          <div class="flex gap-3"><button class="button" [disabled]="form.invalid || busy()">{{ busy() ? 'Saving…' : 'Save user' }}</button>
            <button type="button" class="button secondary" (click)="editing.set(false)" [disabled]="busy()">Cancel</button></div>
        </form>
      </section>
    }
    @if (deleting(); as user) {
      <section class="panel mt-6 border-red-200" role="alert" aria-labelledby="delete-user-title">
        <h2 id="delete-user-title" class="text-xl font-black">Delete {{ user.username }}?</h2>
        <p class="my-4">This removes their access and hides their posts and applications.</p>
        <div class="flex gap-3"><button class="button danger" (click)="confirmDelete()" [disabled]="busy()">{{ busy() ? 'Deleting…' : 'Delete user' }}</button>
          <button class="button secondary" (click)="deleting.set(null)" [disabled]="busy()">Cancel</button></div>
      </section>
    }
    <section class="panel mt-6">
      <div class="mb-5 flex flex-wrap gap-3">
        <label class="grow"><span class="sr-only">Search users</span><input placeholder="Search username or email" [ngModel]="search()" (ngModelChange)="search.set($event)"></label>
        <button class="button secondary" (click)="load()" [disabled]="loading() || busy()">Refresh</button>
      </div>
      @if (loading()) { <p role="status">Loading users…</p> }
      @else {
        <p class="help mb-3">{{ filtered().length }} users</p>
        <div class="overflow-x-auto"><table class="w-full">
          <thead><tr><th>Username</th><th>Email</th><th>Role</th><th>Status</th><th>Actions</th></tr></thead>
          <tbody>@for (user of filtered(); track user.id) {
            <tr><td class="font-bold">{{ user.username }}</td><td>{{ user.email }}</td><td>{{ user.role }}</td><td>{{ user.isActive ? 'Active' : 'Inactive' }}</td>
              <td><div class="flex gap-2">
                <button class="button secondary" (click)="edit(user)" [disabled]="busy()" [attr.aria-label]="'Edit ' + user.username">Edit</button>
                <button class="button secondary" (click)="askDelete(user)" [disabled]="busy() || user.id === auth.user()?.id" [attr.aria-label]="'Delete ' + user.username">Delete</button>
              </div></td></tr>
          } @empty { <tr><td colspan="5">No users found.</td></tr> }</tbody>
        </table></div>
      }
    </section>`
})
export class AdminUsersPage {
  private service = inject(AdminService);
  readonly auth = inject(AuthService);
  users = signal<AdminUser[]>([]); roles = signal<Role[]>([]);
  loading = signal(false); busy = signal(false); error = signal(''); success = signal('');
  search = signal(''); editing = signal(false); deleting = signal<AdminUser | null>(null);
  editId: number | null = null;
  draft: UserInput = this.emptyDraft();
  filtered = computed(() => {
    const term = this.search().trim().toLowerCase();
    return this.users().filter(user => (user.username + ' ' + user.email).toLowerCase().includes(term));
  });
  ngOnInit() { void this.load(); }
  isSelf() { return this.editId === this.auth.user()?.id; }
  private emptyDraft(): UserInput { return { username: '', email: '', password: '', roleId: 0, isActive: true }; }
  create() {
    this.editId = null; this.draft = this.emptyDraft();
    this.draft.roleId = this.roles().find(role => role.name.toLowerCase() === 'user')?.id ?? this.roles()[0].id;
    this.error.set(''); this.success.set(''); this.deleting.set(null); this.editing.set(true);
  }
  edit(user: AdminUser) {
    this.editId = user.id;
    this.draft = { username: user.username, email: user.email, password: '', roleId: user.roleId, isActive: user.isActive };
    this.error.set(''); this.success.set(''); this.deleting.set(null); this.editing.set(true);
  }
  askDelete(user: AdminUser) { this.editing.set(false); this.error.set(''); this.success.set(''); this.deleting.set(user); }
  async load() {
    if (this.loading()) return;
    this.loading.set(true); this.error.set('');
    try {
      const [users, roles] = await Promise.all([this.service.getUsers(), this.service.getRoles()]);
      this.users.set(users); this.roles.set(roles);
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.loading.set(false); }
  }
  async save() {
    if (this.busy()) return;
    this.busy.set(true); this.error.set(''); this.success.set('');
    const self = this.isSelf();
    try {
      const input = { ...this.draft, username: this.draft.username.trim(), email: this.draft.email.trim(), password: this.draft.password || undefined };
      if (this.editId === null) await this.service.createUser(input);
      else await this.service.updateUser(this.editId, input);
      this.editing.set(false); this.draft.password = '';
      if (self) { this.auth.logout(); return; }
      this.success.set('User saved.');
      await this.load();
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
  async confirmDelete() {
    const user = this.deleting();
    if (!user || this.busy()) return;
    this.busy.set(true); this.error.set(''); this.success.set('');
    try {
      await this.service.deleteUser(user.id);
      this.deleting.set(null); this.success.set('User deleted.'); await this.load();
    } catch (error) { this.error.set(errorMessage(error)); }
    finally { this.busy.set(false); }
  }
}
