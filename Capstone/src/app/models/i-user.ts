export interface iUser {
  userId: number;
  name?: string;
  email: string;
  password?: string;
  confirmPassword?: string;
  roles?: string[];
}
