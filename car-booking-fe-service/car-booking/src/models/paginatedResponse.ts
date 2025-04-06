export interface PaginatedResponse<T> {
    status?: string;
    code: number;
    data: T;
    message?: string;
    page_index: number;
    page_size: number;
    total_data: number;
    total_pages: number;
  }