import { inject, Injectable } from '@angular/core';
import { API_BASE_URL } from '../consts/consts';
import { AuthService } from './auth-service';

export class ApiError extends Error {
  constructor(public readonly status: number, message: string) { super(message); }
}
export function errorMessage(error: unknown): string {
  return error instanceof Error ? error.message : 'Something went wrong. Please try again.';
}
export function responseMessage(body: unknown, status: number): string {
  if (Array.isArray(body)) {
    const messages = body.map(item => item?.error).filter(item => typeof item === 'string');
    if (messages.length) return messages.join(' ');
  }
  if (body && typeof body === 'object') {
    const data = body as Record<string, unknown>;
    if (data['errors'] && typeof data['errors'] === 'object') {
      const messages = Object.values(data['errors']).flat().filter(item => typeof item === 'string');
      if (messages.length) return messages.join(' ');
    }
    for (const key of ['message', 'detail', 'title']) {
      if (typeof data[key] === 'string') return data[key];
    }
  }
  return ({ 401: 'Please sign in again.', 403: 'You do not have permission to do this.',
    413: 'The file is too large. Upload a CV up to 5 MB.',
    404: 'The requested record was not found.', 409: 'This change conflicts with an existing record.' } as Record<number, string>)[status]
    ?? 'The request failed. Please try again.';
}
@Injectable({ providedIn: 'root' })
export class ApiService {
  private auth = inject(AuthService);
  async request<T>(path: string, options: RequestInit = {}, authenticated = true): Promise<T> {
    const token = authenticated ? this.auth.getToken() : null;
    const headers = new Headers(options.headers);
    headers.set('Accept', 'application/json');
    if (typeof options.body === 'string') headers.set('Content-Type', 'application/json');
    if (token) headers.set('Authorization', 'Bearer ' + token);
    let response: Response;
    try { response = await fetch(API_BASE_URL + path, { ...options, headers }); }
    catch { throw new ApiError(0, 'Cannot reach the server. Check your connection and try again.'); }
    const raw = await response.text();
    let body: unknown;
    try { body = raw ? JSON.parse(raw) : undefined; } catch { body = undefined; }
    if (!response.ok) {
      // An old request must not log out a newly signed-in session.
      if (response.status === 401 && token && this.auth.getToken() === token) this.auth.logout(true);
      throw new ApiError(response.status, responseMessage(body, response.status));
    }
    if (raw && body === undefined) throw new ApiError(response.status, 'The server returned an invalid response.');
    return body as T;
  }
}
