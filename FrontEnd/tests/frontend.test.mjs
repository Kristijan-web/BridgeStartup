import '@angular/compiler';
import { test, afterEach } from 'node:test';
import assert from 'node:assert/strict';
import { createEnvironmentInjector, runInInjectionContext, signal } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../tmp/frontend-tests/src/app/services/auth-service.mjs';
import { ApiService, ApiError } from '../tmp/frontend-tests/src/app/services/api-service.mjs';
import { SESSION_KEY, validSession, safeReturnUrl } from '../tmp/frontend-tests/src/app/services/session.mjs';
import { adminGuard, authGuard } from '../tmp/frontend-tests/src/app/guards/auth-guards.mjs';
import { AdminService } from '../tmp/frontend-tests/src/app/services/user-service.mjs';
import { AdminUsersPage } from '../tmp/frontend-tests/src/app/pages/admin-users-page.mjs';
import { AdminPostsPage } from '../tmp/frontend-tests/src/app/pages/admin-posts-page.mjs';
import { PostsService } from '../tmp/frontend-tests/src/app/services/posts-service.mjs';
import { validateCv } from '../tmp/frontend-tests/src/app/components/post-application-form.mjs';
import { ContactsService } from '../tmp/frontend-tests/src/app/services/contacts-service.mjs';
import { ContactForm } from '../tmp/frontend-tests/src/app/components/contact-form.mjs';

const originalFetch = globalThis.fetch;
const stores = new Map();
const listeners = new Map();
globalThis.localStorage = {
  getItem: key => stores.get(key) ?? null,
  setItem: (key, value) => stores.set(key, value),
  removeItem: key => stores.delete(key)
};
globalThis.window = { addEventListener: (name, fn) => listeners.set(name, fn) };
const token = (exp = Date.now() / 1000 + 60) => 'header.' + Buffer.from(JSON.stringify({ exp })).toString('base64url') + '.signature';
const session = role => ({ token: token(), user: { id: 1, username: 'Admin', email: 'admin@example.test', role } });
const injectors = [];
const auths = [];
function setup(providers = []) {
  const calls = [];
  const router = { url: '/admin/users', navigate: async (...args) => calls.push(args),
    navigateByUrl: async url => calls.push(url), createUrlTree: (commands, extras) => ({ commands, extras }) };
  const injector = createEnvironmentInjector([{ provide: Router, useValue: router }, AuthService, ApiService, ...providers]);
  injectors.push(injector);
  return { injector, calls, router };
}
function getAuth(injector) { const auth = injector.get(AuthService); auths.push(auth); return auth; }
afterEach(() => {
  for (const auth of auths.splice(0)) auth.logout();
  for (const injector of injectors.splice(0)) injector.destroy();
  stores.clear(); listeners.clear(); globalThis.fetch = originalFetch;
});

test('contact submission sends the current user and trimmed fields, waits for success, and blocks duplicates', async () => {
  const { injector } = setup([ContactsService]); getAuth(injector).establish(session('user'));
  const component = runInInjectionContext(injector, () => new ContactForm());
  component.subject = ' Partnership '; component.message = ' First line\nSecond line ';
  let complete, calls = 0, reset;
  const form = { invalid: false, resetForm: value => { reset = value; } };
  globalThis.fetch = async (url, options) => {
    calls++;
    assert.equal(url, '/api/contacts'); assert.equal(options.method, 'POST');
    assert.match(options.headers.get('Authorization'), /^Bearer /);
    assert.equal(options.headers.get('Content-Type'), 'application/json');
    assert.deepEqual(JSON.parse(options.body), { userId: 1, subject: 'Partnership', message: 'First line\nSecond line' });
    return new Promise(resolve => { complete = resolve; });
  };
  const request = component.submit(form);
  assert.equal(component.busy(), true); assert.equal(component.success(), false);
  await component.submit(form); assert.equal(calls, 1);
  complete(new Response(null, { status: 201 })); await request;
  assert.equal(component.busy(), false); assert.equal(component.success(), true);
  assert.deepEqual(reset, { subject: '', message: '' });
  assert.equal(component.subject, ''); assert.equal(component.message, '');
});

test('contact submission rejects guests, invalid forms, and whitespace-only messages without an API call', async () => {
  const { injector } = setup([ContactsService]); const auth = getAuth(injector);
  const component = runInInjectionContext(injector, () => new ContactForm());
  let touched = 0, calls = 0;
  const form = { invalid: false, control: { markAllAsTouched: () => touched++ } };
  globalThis.fetch = async () => { calls++; throw new Error('Unexpected request'); };
  component.subject = 'Subject'; component.message = 'Message';
  await component.submit(form); assert.match(component.error(), /sign in/);
  auth.establish(session('user'));
  component.message = ' \n\t '; await component.submit(form);
  component.message = 'Message'; form.invalid = true; await component.submit(form);
  assert.equal(touched, 2); assert.equal(calls, 0); assert.equal(component.busy(), false);
});

test('contact errors preserve the draft and permit retry after server and network failures', async () => {
  const { injector } = setup([ContactsService]); getAuth(injector).establish(session('user'));
  const component = runInInjectionContext(injector, () => new ContactForm());
  component.subject = 'Help'; component.message = 'Please help with my post.';
  const form = { invalid: false, resetForm: () => {} };
  globalThis.fetch = async () => new Response('{"errors":{"Subject":["Subject is too long."]}}', { status: 400 });
  await component.submit(form);
  assert.equal(component.error(), 'Subject is too long.');
  assert.equal(component.subject, 'Help'); assert.equal(component.success(), false); assert.equal(component.busy(), false);
  globalThis.fetch = async () => { throw new TypeError('Failed to fetch'); };
  await component.submit(form); assert.match(component.error(), /Cannot reach the server/);
  assert.equal(component.message, 'Please help with my post.');
  globalThis.fetch = async () => new Response(null, { status: 201 });
  await component.submit(form); assert.equal(component.error(), ''); assert.equal(component.success(), true);
});

test('saved sessions reject expired, malformed and incomplete login data', () => {
  assert.equal(validSession(session('admin')), true);
  assert.equal(validSession({ ...session('admin'), token: token(1) }), false);
  assert.equal(validSession({ token: 'bad', user: {} }), false);
  stores.set(SESSION_KEY, '{invalid');
  const { injector } = setup();
  assert.equal(getAuth(injector).getToken(), null);
  assert.equal(stores.has(SESSION_KEY), false);
});

test('logout clears credentials and admin state and redirects to login', () => {
  const { injector, calls } = setup();
  const auth = getAuth(injector);
  auth.establish(session('admin'));
  assert.equal(auth.isAdmin(), true);
  auth.logout();
  assert.equal(auth.user(), null);
  assert.equal(auth.getToken(), null);
  assert.equal(stores.has(SESSION_KEY), false);
  assert.deepEqual(calls.at(-1)[0], ['/login']);
});

test('logout in another tab clears this tab and leaves protected pages', () => {
  const { injector, calls } = setup();
  const auth = getAuth(injector); auth.establish(session('admin'));
  stores.delete(SESSION_KEY);
  listeners.get('storage')({ key: SESSION_KEY });
  assert.equal(auth.user(), null);
  assert.equal(calls.at(-1), '/');
});

test('an expired session cannot attach its token to a new request', () => {
  const { injector, calls } = setup();
  const auth = getAuth(injector); auth.establish(session('admin'));
  const originalNow = Date.now;
  try {
    Date.now = () => originalNow() + 120000;
    assert.equal(auth.getToken(), null);
    assert.equal(auth.user(), null);
    assert.deepEqual(calls.at(-1)[1], { queryParams: { expired: '1' } });
  } finally { Date.now = originalNow; }
});

test('admin guard distinguishes guest, regular user, and admin', () => {
  const { injector } = setup();
  const auth = getAuth(injector);
  const guard = () => runInInjectionContext(injector, () => adminGuard({}, { url: '/admin/posts' }));
  assert.deepEqual(guard(), { commands: ['/login'], extras: { queryParams: { returnUrl: '/admin/posts' } } });
  auth.establish(session('user'));
  assert.deepEqual(guard().commands, ['/forbidden']);
  auth.establish(session('admin'));
  assert.equal(guard(), true);
});

test('return URLs stay inside the application', () => {
  for (const url of ['https://evil.test', '//evil.test', '/\\evil.test', '/login?returnUrl=x', null]) {
    assert.equal(safeReturnUrl(url), '/');
  }
  assert.equal(safeReturnUrl('/admin/posts'), '/admin/posts');
});

test('API sends bearer and JSON headers and handles empty 204 responses', async () => {
  const { injector } = setup(); getAuth(injector).establish(session('admin'));
  globalThis.fetch = async (url, options) => {
    assert.equal(url, '/api/admin/users/4');
    assert.equal(options.method, 'PUT');
    assert.match(options.headers.get('Authorization'), /^Bearer /);
    assert.equal(options.headers.get('Content-Type'), 'application/json');
    return new Response(null, { status: 204 });
  };
  assert.equal(await injector.get(ApiService).request('/admin/users/4', { method: 'PUT', body: '{}' }), undefined);
});

test('failed login does not attach or invalidate another saved session', async () => {
  const { injector } = setup(); const auth = getAuth(injector); auth.establish(session('admin'));
  globalThis.fetch = async (_url, options) => {
    assert.equal(options.headers.has('Authorization'), false);
    return new Response(null, { status: 401 });
  };
  await assert.rejects(injector.get(ApiService).request('/Auth/login', {}, false), ApiError);
  assert.equal(auth.isAdmin(), true);
});

test('authenticated 401 clears session, while 403 preserves it', async () => {
  const { injector } = setup(); const auth = getAuth(injector); auth.establish(session('admin'));
  globalThis.fetch = async () => new Response(null, { status: 403 });
  await assert.rejects(injector.get(ApiService).request('/admin/users'), /permission/);
  assert.equal(auth.isAdmin(), true);
  globalThis.fetch = async () => new Response(null, { status: 401 });
  await assert.rejects(injector.get(ApiService).request('/admin/users'), /sign in/);
  assert.equal(auth.user(), null);
});

test('ASP.NET validation errors and network failures remain visible', async () => {
  const { injector } = setup(); getAuth(injector);
  for (const body of [[{ property: 'Email', error: 'Email already exists' }], { errors: { Email: ['Email already exists'] } }]) {
    globalThis.fetch = async () => new Response(JSON.stringify(body), { status: 422 });
    await assert.rejects(injector.get(ApiService).request('/Auth/register'), /Email already exists/);
  }
  globalThis.fetch = async () => { throw new TypeError('fetch failed'); };
  await assert.rejects(injector.get(ApiService).request('/Posts'), /Cannot reach/);
});

test('user edit omits an empty password and failed deletes keep confirmation open', async () => {
  let saved;
  const service = { updateUser: async (id, input) => { saved = { id, input }; },
    getUsers: async () => [], getRoles: async () => [],
    deleteUser: async () => { throw new Error('Cannot delete'); } };
  const { injector } = setup([{ provide: AdminService, useValue: service }]);
  getAuth(injector).establish(session('admin'));
  const page = runInInjectionContext(injector, () => new AdminUsersPage());
  const user = { id: 2, username: 'Founder', email: 'founder@example.test', role: 'user', roleId: 2, isActive: true };
  page.edit(user);
  await page.save();
  assert.equal(saved.id, 2);
  assert.equal(saved.input.password, undefined);
  assert.equal(page.editing(), false);
  page.askDelete(user);
  await page.confirmDelete();
  assert.equal(page.deleting().id, 2);
  assert.equal(page.error(), 'Cannot delete');
});

test('post edits can remove every badge and clear optional contact fields', async () => {
  let saved;
  const service = { updatePost: async (id, input) => { saved = { id, input }; }, getUsers: async () => [], getPosts: async () => [] };
  const { injector } = setup([{ provide: AdminService, useValue: service }]);
  const page = runInInjectionContext(injector, () => new AdminPostsPage());
  page.edit({ id: 3, title: 'Idea', description: 'Details', userId: 2, badges: ['C#'], email: 'x@example.test', phone: '123' });
  page.badges = ''; page.draft.email = ''; page.draft.phone = '';
  await page.save();
  assert.deepEqual(saved.input.badges, []);
  assert.equal(saved.input.email, null);
  assert.equal(saved.input.phone, null);
  assert.equal(page.editing(), false);
});

test('post creation waits for the server and prevents duplicate submissions', async () => {
  let resolve; let count = 0;
  const service = { createPost: () => { count++; return new Promise(done => resolve = done); },
    getUsers: async () => [], getPosts: async () => [] };
  const { injector } = setup([{ provide: AdminService, useValue: service }]);
  const page = runInInjectionContext(injector, () => new AdminPostsPage());
  page.draft = { title: 'New idea', description: 'Details', userId: 2, email: '', phone: '', badges: [] };
  page.editing.set(true);
  const first = page.save(); await page.save();
  assert.equal(count, 1); assert.equal(page.editing(), true);
  resolve(); await first;
  assert.equal(page.editing(), false); assert.equal(page.busy(), false);
});

test('post sorting and filtering use the existing ASP.NET query parameters', async () => {
  const { injector } = setup([PostsService]); getAuth(injector);
  globalThis.fetch = async url => {
    const parsed = new URL(url, 'http://localhost');
    assert.equal(parsed.pathname, '/api/Posts');
    assert.equal(parsed.searchParams.get('Title'), 'C# & design');
    assert.equal(parsed.searchParams.get('SortBy'), 'title');
    assert.equal(parsed.searchParams.get('SortOrder'), 'desc');
    assert.equal(parsed.searchParams.get('Page'), '2');
    return new Response('[]');
  };
  assert.deepEqual(await injector.get(PostsService).getAllPosts(' C# & design ', 'desc', 2), []);
});

test('application sends the selected post and CV as multipart without overriding its boundary', async () => {
  const { injector } = setup([PostsService]); getAuth(injector).establish(session('user'));
  const cv = new File(['%PDF-1.4 resume'], 'my.resume.pdf', { type: 'application/pdf' });
  globalThis.fetch = async (url, options) => {
    assert.equal(url, '/api/Posts/apply');
    assert.equal(options.method, 'POST');
    assert.equal(options.headers.has('Content-Type'), false);
    assert.match(options.headers.get('Authorization'), /^Bearer /);
    assert.equal(options.body.get('PostId'), '42');
    assert.equal(options.body.has('UserId'), false);
    assert.equal(options.body.get('userFile').name, 'my.resume.pdf');
    assert.equal(await options.body.get('userFile').text(), '%PDF-1.4 resume');
    return new Response(null, { status: 204 });
  };
  await injector.get(PostsService).applyToPost(42, cv);
});

test('admin CRUD uses implemented endpoints and PATCH Users for profile edits', async () => {
  const { injector } = setup([AdminService, PostsService]); getAuth(injector).establish(session('admin'));
  const requests = [];
  globalThis.fetch = async (url, options) => {
    requests.push([new URL(url, 'http://localhost').pathname, options.method ?? 'GET']);
    if (url === '/api/admin/users/2' && !options.method) return new Response('{"roleId":1,"isActive":true}');
    return new Response(options.method === 'GET' || !options.method ? '[]' : null,
      { status: options.method === 'GET' || !options.method ? 200 : 204 });
  };
  const service = injector.get(AdminService);
  await service.getUsers(); await service.getPosts('desc');
  await service.createUser({}); await service.updateUser(2, { roleId: 1, isActive: true }); await service.deleteUser(2);
  await service.createPost({}); await service.updatePost(3, {}); await service.deletePost(3);
  assert.deepEqual(requests, [
    ['/api/admin/users', 'GET'], ['/api/Posts', 'GET'], ['/api/admin/posts', 'GET'],
    ['/api/admin/users', 'POST'], ['/api/admin/users/2', 'GET'], ['/api/Users/2', 'PATCH'],
    ['/api/Users/2', 'DELETE'], ['/api/admin/posts', 'POST'], ['/api/admin/posts/3', 'PUT'], ['/api/Posts/3', 'DELETE']
  ]);
});

test('role and activation edits use the admin endpoint that supports those fields', async () => {
  const { injector } = setup([AdminService, PostsService]); getAuth(injector).establish(session('admin'));
  const writes = [];
  globalThis.fetch = async (url, options) => {
    if (!options.method) return new Response('{"roleId":1,"isActive":true}');
    writes.push({ url, method: options.method, body: JSON.parse(options.body) });
    return new Response(null, { status: 204 });
  };
  const service = injector.get(AdminService);
  await service.updateUser(2, { roleId: 2, isActive: true });
  await service.updateUser(2, { roleId: 1, isActive: false });
  assert.deepEqual(writes, [
    { url: '/api/admin/users/2', method: 'PUT', body: { roleId: 2, isActive: true } },
    { url: '/api/admin/users/2', method: 'PUT', body: { roleId: 1, isActive: false } }
  ]);
});

test('CV validation rejects empty, oversized, and unsupported files', () => {
  assert.match(validateCv(new File([], 'cv.pdf')), /non-empty/);
  assert.match(validateCv(new File(['test'], 'cv.exe')), /PDF/);
  assert.match(validateCv(new File([new Uint8Array(5 * 1024 * 1024 + 1)], 'cv.pdf')), /5 MB/);
  assert.equal(validateCv(new File(['%PDF-1.4'], 'my.resume.PDF')), '');
});

test('my posts requires login but accepts a regular user and preserves the return URL', () => {
  const { injector } = setup(); const auth = getAuth(injector);
  const result = runInInjectionContext(injector, () => authGuard({}, { url: '/my-posts?create=1' }));
  assert.deepEqual(result, { commands: ['/login'], extras: { queryParams: { returnUrl: '/my-posts?create=1' } } });
  auth.establish(session('user'));
  assert.equal(runInInjectionContext(injector, () => authGuard({}, { url: '/my-posts' })), true);
});

test('user publishing sends numeric badges and no client-controlled owner ID', async () => {
  const { injector } = setup([PostsService]); getAuth(injector).establish(session('user'));
  globalThis.fetch = async (url, options) => {
    assert.equal(url, '/api/Posts'); assert.equal(options.method, 'POST');
    const body = JSON.parse(options.body);
    assert.deepEqual(body.badges, [2, 5]); assert.equal(body.userId, undefined);
    return new Response(null, { status: 201 });
  };
  await injector.get(PostsService).createPost({ title: 'My idea', description: 'A useful new project', email: 'user@example.test', phone: '123', badges: [2, 5] });
});

test('CV download uses the selected post and applicant with a bearer token and keeps binary content', async () => {
  const { injector } = setup([PostsService]); getAuth(injector).establish(session('user'));
  const bytes = new Uint8Array([37, 80, 68, 70, 0, 255, 254, 1]);
  globalThis.fetch = async (url, options) => {
    assert.equal(url, '/api/PostApplications/42/7/file');
    assert.match(options.headers.get('Authorization'), /^Bearer /);
    assert.equal(options.headers.get('Accept'), 'application/octet-stream');
    return new Response(bytes, { headers: { 'Content-Type': 'application/pdf' } });
  };
  const blob = await injector.get(PostsService).downloadCv(42, 7);
  assert.deepEqual(new Uint8Array(await blob.arrayBuffer()), bytes);
});

test('download errors stay visible and an expired download signs the user out', async () => {
  const { injector } = setup([PostsService]); const auth = getAuth(injector); auth.establish(session('user'));
  globalThis.fetch = async () => new Response('{"message":"This CV is no longer available."}', { status: 404 });
  await assert.rejects(injector.get(PostsService).downloadCv(42, 7), /no longer available/);
  assert.ok(auth.getToken());
  globalThis.fetch = async () => new Response('', { status: 401 });
  await assert.rejects(injector.get(PostsService).downloadCv(42, 7), /sign in/);
  assert.equal(auth.getToken(), null);
});
