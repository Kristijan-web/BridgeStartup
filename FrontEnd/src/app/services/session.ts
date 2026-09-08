import { UserInterface } from '../interfaces/user-interface';

export interface Session { user: UserInterface; token: string }
export const SESSION_KEY = 'bridgestartup.session';

export function tokenExpiry(token: string): number {
  try {
    const part = token.split('.')[1];
    const payload = JSON.parse(atob(part.replace(/-/g, '+').replace(/_/g, '/')));
    return typeof payload.exp === 'number' ? payload.exp * 1000 : 0;
  } catch { return 0; }
}
export function validSession(value: unknown): value is Session {
  if (!value || typeof value !== 'object') return false;
  const { user, token } = value as Session;
  return typeof token === 'string' && tokenExpiry(token) > Date.now()
    && !!user && Number.isSafeInteger(user.id) && user.id > 0
    && typeof user.username === 'string' && typeof user.email === 'string'
    && typeof user.role === 'string';
}
export function safeReturnUrl(value: string | null): string {
  return value?.startsWith('/') && !value.startsWith('//') && !value.includes('\\')
    && !/^\/(login|register)([/?#]|$)/.test(value) ? value : '/';
}
