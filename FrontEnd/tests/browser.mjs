import assert from 'node:assert/strict';
import { createServer } from 'node:http';
import { spawn } from 'node:child_process';
import { readFile, writeFile, mkdir, mkdtemp } from 'node:fs/promises';
import path from 'node:path';

const root = path.resolve('dist/FrontEnd/browser');
const roles = [{ id: 1, name: 'user' }, { id: 2, name: 'admin' }];
let users = [
  { id: 1, username: 'Admin', email: 'admin@example.test', role: 'admin', roleId: 2, isActive: true },
  { id: 2, username: 'Founder', email: 'founder@example.test', role: 'user', roleId: 1, isActive: true }
];
let posts = [{ id: 1, title: 'Startup idea', description: 'A useful project.', email: null, phone: null,
  userId: 2, user: users[1], badges: ['TypeScript'] }];
const apiRequests = [];
const applications = new Set();
const server = createServer(async (req, res) => {
  try {
    const url = new URL(req.url, 'http://localhost');
    if (url.pathname.startsWith('/api/')) {
      let raw = '';
      for await (const chunk of req) raw += chunk;
      const multipart = req.headers['content-type']?.startsWith('multipart/form-data');
      const body = raw && !multipart ? JSON.parse(raw) : undefined;
      apiRequests.push({ path: url.pathname, method: req.method, query: url.search, body });
      const json = (data, status = 200) => {
        res.writeHead(status, { 'Content-Type': 'application/json' });
        res.end(data === undefined ? undefined : JSON.stringify(data));
      };
      if (url.pathname === '/api/Auth/login') return json({ user: users[0],
        token: 'header.' + Buffer.from(JSON.stringify({ exp: Date.now() / 1000 + 3600 })).toString('base64url') + '.signature' });
      if (url.pathname === '/api/admin/roles') return json(roles);
      if (url.pathname === '/api/Posts' && multipart) {
        const data = await new Request('http://localhost/api/Posts', { method: 'POST', headers: { 'Content-Type': req.headers['content-type'] }, body: raw }).formData();
        apiRequests.at(-1).body = { postId: data.get('PostId'), fileName: data.get('userFile').name };
        if (applications.has(data.get('PostId'))) return json({ message: 'You have already applied to this post.' }, 409);
        applications.add(data.get('PostId'));
        return json(undefined, 204);
      }
      if (url.pathname === '/api/Posts' && req.method === 'GET') {
        const result = posts.filter(p => p.title.toLowerCase().includes((url.searchParams.get('Title') ?? '').toLowerCase()))
          .toSorted((a, b) => a.title.localeCompare(b.title) * (url.searchParams.get('SortOrder') === 'desc' ? -1 : 1));
        return json(result);
      }
      if (/^\/api\/Posts\/\d+$/.test(url.pathname) && req.method === 'GET') return json(posts.find(p => p.id === +url.pathname.split('/').at(-1)));
      const match = /^\/api\/(Users|Posts)(?:\/(\d+))?$/.exec(url.pathname);
      if (!match) return json({ message: 'Unknown endpoint' }, 404);
      const kind = match[1].toLowerCase(), id = Number(match[2]);
      const records = kind === 'users' ? users : posts;
      if (req.method === 'GET') return json(records);
      if (req.method === 'DELETE') {
        if (kind === 'users') users = users.filter(x => x.id !== id);
        else posts = posts.filter(x => x.id !== id);
        return json(undefined, 204);
      }
      const value = kind === 'users'
        ? { ...body, role: roles.find(r => r.id === body.roleId).name }
        : { ...body, user: users.find(u => u.id === body.userId) };
      if (req.method === 'POST') {
        const record = { ...value, id: Math.max(0, ...records.map(x => x.id)) + 1 };
        records.push(record); return json(record, 201);
      }
      Object.assign(records.find(x => x.id === id), value);
      return json(undefined, 204);
    }
    const requested = path.resolve(root, '.' + decodeURIComponent(url.pathname));
    if (!requested.startsWith(root + path.sep) && requested !== root) { res.writeHead(403); res.end(); return; }
    let file = requested;
    try { if (!path.extname(file)) file = path.join(root, 'index.html'); await readFile(file); }
    catch { file = path.join(root, 'index.html'); }
    const type = { '.html': 'text/html', '.js': 'text/javascript', '.css': 'text/css', '.svg': 'image/svg+xml', '.png': 'image/png' }[path.extname(file)] ?? 'application/octet-stream';
    res.writeHead(200, { 'Content-Type': type }); res.end(await readFile(file));
  } catch (error) { res.writeHead(500); res.end(String(error)); }
});
await new Promise(resolve => server.listen(0, '127.0.0.1', resolve));
const origin = 'http://127.0.0.1:' + server.address().port;
await mkdir('tmp', { recursive: true });
const profile = await mkdtemp(path.resolve('tmp/browser-check-'));
const executable = process.env.BROWSER_PATH ?? 'C:/Program Files (x86)/Microsoft/Edge/Application/msedge.exe';
const browser = spawn(executable, ['--headless=new', '--disable-gpu', '--no-first-run', '--no-default-browser-check',
  '--remote-debugging-port=0', '--user-data-dir=' + profile, 'about:blank'], { stdio: 'ignore', windowsHide: true });
let socket;
const pending = new Map();
let sequence = 0;
const errors = [];
const pause = ms => new Promise(resolve => setTimeout(resolve, ms));
async function until(predicate, label) {
  for (let i = 0; i < 100; i++) { if (await predicate()) return; await pause(100); }
  throw new Error('Timed out: ' + label);
}
function command(method, params = {}) {
  const id = ++sequence;
  return new Promise((resolve, reject) => {
    pending.set(id, { resolve, reject });
    socket.send(JSON.stringify({ id, method, params }));
  });
}
async function evaluate(expression) {
  const result = await command('Runtime.evaluate', { expression, returnByValue: true, awaitPromise: true });
  if (result.exceptionDetails) throw new Error(result.exceptionDetails.text);
  return result.result.value;
}
async function waitFor(expression) { await until(() => evaluate(expression), expression); }
const text = value => JSON.stringify(value);
async function fill(name, value) {
  await evaluate("(() => { const e = document.querySelector('[name=" + name + "]'); e.value = " + text(value) + "; e.dispatchEvent(new Event('input', { bubbles: true })); })()");
}
async function click(label) {
  const target = "[...document.querySelectorAll('button'), ...document.querySelectorAll('a')].find(e => e.textContent.trim() === " + text(label) + ")";
  await waitFor("(() => { const e = " + target + "; return e && !e.disabled; })()");
  await evaluate(target + ".click()");
}
try {
  let port;
  await until(async () => {
    try { port = (await readFile(path.join(profile, 'DevToolsActivePort'), 'utf8')).split('\n')[0]; return !!port; }
    catch { return false; }
  }, 'browser startup');
  const tab = await (await fetch('http://127.0.0.1:' + port + '/json/new?about:blank', { method: 'PUT' })).json();
  socket = new WebSocket(tab.webSocketDebuggerUrl);
  await new Promise((resolve, reject) => { socket.addEventListener('open', resolve, { once: true }); socket.addEventListener('error', reject, { once: true }); });
  socket.addEventListener('message', event => {
    const message = JSON.parse(event.data);
    if (message.method === 'Runtime.exceptionThrown') errors.push(message.params.exceptionDetails);
    if (message.id) {
      const request = pending.get(message.id); pending.delete(message.id);
      if (message.error) request.reject(new Error(message.error.message)); else request.resolve(message.result);
    }
  });
  await command('Runtime.enable'); await command('Page.enable');
  await command('Page.navigate', { url: origin + '/admin/users' });
  await waitFor("location.pathname === '/login' && !!document.querySelector('[name=email]')");
  await fill('email', 'admin@example.test'); await fill('password', 'AdminPass1'); await click('Sign in');
  await waitFor("location.pathname === '/admin/users' && document.querySelectorAll('tbody tr').length === 2");
  console.log('PASS protected navigation and login');

  await click('Add user'); await fill('username', 'NewFounder'); await fill('email', 'new@example.test'); await fill('password', 'FounderPass1');
  await click('Save user');
  await waitFor("document.querySelectorAll('tbody tr').length === 3");
  await evaluate(`document.querySelector('button[aria-label="Edit NewFounder"]').click()`);
  await fill('username', 'EditedFounder'); await click('Save user');
  await waitFor(`!!document.querySelector('button[aria-label="Edit EditedFounder"]')`);
  assert.equal(apiRequests.findLast(x => x.method === 'PUT' && x.path.includes('/Users/')).body.password, undefined);
  console.log('PASS user create and edit preserve blank passwords');

  await click('Posts'); await waitFor("location.pathname === '/admin/posts' && !!document.querySelector('tbody tr')");
  await click('Add post'); await fill('title', 'Browser idea'); await fill('description', 'Created from the admin form'); await fill('badges', 'C#, TypeScript');
  await click('Save post'); await waitFor("document.querySelectorAll('tbody tr').length === 2");
  await evaluate(`document.querySelector('button[aria-label="Edit Browser idea"]').click()`);
  await fill('badges', ''); await click('Save post');
  await waitFor("!document.querySelector('#post-form-title')");
  assert.deepEqual(apiRequests.findLast(x => x.method === 'PATCH' && x.path.includes('/Posts/')).body.badges, []);
  await evaluate(`(() => { const e = document.querySelector('[name=sortOrder]'); e.value = 'desc'; e.dispatchEvent(new Event('change', { bubbles: true })); })()`);
  await waitFor("!document.querySelector('[name=sortOrder]').disabled");
  assert.match(apiRequests.findLast(x => x.method === 'GET' && x.path === '/api/Posts').query, /SortOrder=desc/);
  await command('Emulation.setDeviceMetricsOverride', { width: 390, height: 844, deviceScaleFactor: 1, mobile: true });
  await waitFor("document.documentElement.scrollWidth <= 390");
  const screenshot = await command('Page.captureScreenshot', { format: 'png' });
  await writeFile('tmp/admin-mobile.png', Buffer.from(screenshot.data, 'base64'));
  await command('Emulation.clearDeviceMetricsOverride');
  await evaluate(`document.querySelector('button[aria-label="Delete Browser idea"]').click()`);
  await click('Delete post'); await waitFor("document.querySelectorAll('tbody tr').length === 1");
  console.log('PASS post CRUD, empty badges, and mobile layout');

  await click('Users'); await waitFor("location.pathname === '/admin/users' && document.querySelectorAll('tbody tr').length === 3");
  await evaluate(`document.querySelector('button[aria-label="Delete EditedFounder"]').click()`);
  await click('Delete user'); await waitFor("document.querySelectorAll('tbody tr').length === 2");
  await click('Log out'); await waitFor("location.pathname === '/login'");
  assert.equal(await evaluate("localStorage.getItem('bridgestartup.session')"), null);
  await command('Page.navigate', { url: origin + '/admin/posts' });
  await waitFor("location.pathname === '/login'");
  await command('Page.navigate', { url: origin + '/posts/1' });
  await waitFor("document.body.innerText.includes('Sign in to apply')");
  assert.equal(await evaluate("!!document.querySelector('a[href^=\"mailto:\"]')"), false);
  await click('Sign in to apply');
  await waitFor("location.pathname === '/login' && !!document.querySelector('[name=email]')");
  await fill('email', 'admin@example.test'); await fill('password', 'AdminPass1'); await click('Sign in');
  await waitFor("location.pathname === '/posts/1' && !!document.querySelector('[name=userFile]')");
  await evaluate(`(() => { const transfer = new DataTransfer(); transfer.items.add(new File(['%PDF-1.4 resume'], 'my.resume.pdf', { type: 'application/pdf' })); const input = document.querySelector('[name=userFile]'); input.files = transfer.files; input.dispatchEvent(new Event('change', { bubbles: true })); })()`);
  await click('Submit application');
  await waitFor("document.body.innerText.includes('Your application has been submitted.')");
  assert.deepEqual(apiRequests.findLast(x => x.method === 'POST' && x.path === '/api/Posts').body, { postId: '1', fileName: 'my.resume.pdf' });
  await command('Page.navigate', { url: origin + '/posts/1' });
  await waitFor("!!document.querySelector('[name=userFile]')");
  await evaluate(`(() => { const transfer = new DataTransfer(); transfer.items.add(new File(['%PDF-1.4 resume'], 'resume.pdf')); const input = document.querySelector('[name=userFile]'); input.files = transfer.files; input.dispatchEvent(new Event('change', { bubbles: true })); })()`);
  await click('Submit application');
  await waitFor("document.body.innerText.includes('You have already applied to this post.')");
  const applicationScreenshot = await command('Page.captureScreenshot', { format: 'png' });
  await writeFile('tmp/post-application.png', Buffer.from(applicationScreenshot.data, 'base64'));
  console.log('PASS server sorting, return-to-post login, multipart CV application, and duplicate handling');
  assert.deepEqual(errors, []);
  console.log('PASS user deletion, logout, and protected navigation after logout');
  console.log('Browser checks passed with no runtime exceptions. Screenshot: tmp/admin-mobile.png');
} catch (error) {
  if (socket?.readyState === WebSocket.OPEN) {
    console.error(await evaluate('JSON.stringify({ url: location.href, text: document.body.innerText })'));
    console.error('Browser errors:', JSON.stringify(errors));
    console.error('Requests:', apiRequests.map(({ path, method }) => ({ path, method })));
  }
  throw error;
} finally {
  if (socket?.readyState === WebSocket.OPEN) {
    try { await command('Browser.close'); } catch {}
    socket.close();
  }
  browser.kill();
  server.closeAllConnections();
  await new Promise(resolve => server.close(resolve));
}
