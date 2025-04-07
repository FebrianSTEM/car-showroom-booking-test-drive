export interface GetPagingBookingRequest {
    start_date: string;
    end_date: string;
    customer_name?: string;
    customer_phone?: string;
    customer_email?: string;
    car_id?: number;
    car_brand?: string;
    car_model?: string;
    car_year?: number;
  }