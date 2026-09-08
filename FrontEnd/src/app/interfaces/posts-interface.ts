export interface PostsInterface {
  id: number;
  userId: number;
  title: string;
  description: string;
  email: string | null;
  phone: string | null;
  user: { username: string; email: string };
  badges: string[];
}
export interface AdminPost extends PostsInterface { userId: number }
export interface PostInput {
  title: string;
  description: string;
  email: string | null;
  phone: string | null;
  userId: number;
  badges: string[];
}
