export interface UpdateCarModelRequest {
    car_id: number;
    brand: string;
    model: string;
    year: number;
    image_url: string;
    description: string;
}