import { UserResponse } from "./userResponse";

export interface AuthResponse {
    token?: string;
    user: UserResponse;
  }