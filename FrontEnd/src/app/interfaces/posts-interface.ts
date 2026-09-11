export interface PostsInterface {
  id: number;
  userId?: number;
  title: string;
  description: string;
  email: string | null;
  phone: string | null;
  user: { username: string; email: string };
  badges: string[];
}
export interface AdminPost extends PostsInterface { userId: number }

export interface BadgeChoice { id: number; name: string }

export interface OwnedPost { id: number; title: string; applicationCount: number }

export interface PostApplicant { userId: number; username: string; createdAt: string; fileName: string }

export interface CreatePostInput {
  title: string;
  description: string;
  email: string;
  phone: string;
  badges: number[];
}
export interface PostInput {
  title: string;
  description: string;
  email: string | null;
  phone: string | null;
  userId: number;
  badges: string[];
}
