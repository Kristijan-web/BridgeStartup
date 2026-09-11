import { UserInterface } from './user-interface';

export interface CreateContactInput {
  userId: number;
  subject: string;
  message: string;
}

export interface Contact {
  id: number;
  subject: string;
  message: string;
  user: UserInterface;
}
