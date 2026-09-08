export interface UserInterface {
  id: number;
  username: string;
  email: string;
  role: string;
}
export interface AdminUser extends UserInterface { roleId: number; isActive: boolean }
export interface UserInput {
  username: string;
  email: string;
  password?: string;
  roleId: number;
  isActive: boolean;
}
export interface Role { id: number; name: string }
