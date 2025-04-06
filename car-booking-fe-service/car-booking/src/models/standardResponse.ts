export interface StandardResponse<T> {
  status?: string;
  code: number;
  data: T;
  message?: string;
}