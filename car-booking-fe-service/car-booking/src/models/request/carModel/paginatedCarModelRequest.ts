export interface PaginatedCarModelRequest {
    brand?: string;
    model?: string;
    year? : number;
    description?: string;
    page: number;
    page_size: number;
  }