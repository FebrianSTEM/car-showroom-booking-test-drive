import axios from 'axios';
import { StandardResponse } from '../../models/standardResponse';
import { CarBookingRequest } from '../../models/request/booking/createBookingRequest';
import { CarBookingResponse } from '../../models/response/booking/bookingResponse';
import { PaginatedResponse } from '../../models/paginatedResponse';
import { GetPagingBookingRequest } from '../../models/request/booking/getPagingBookingRequest';
import { UpdateBookingRequest } from '../../models/request/booking/updateBookingRequest';

const bookingApiConnector = {
    createBooking: async (
        req: CarBookingRequest
    ): Promise<StandardResponse<CarBookingResponse>> => {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.post<StandardResponse<CarBookingResponse>>(
                `/carBooking/Booking`,
                req,
                { headers: { Authorization: `Bearer ${token}` } }
            );
            return response.data;
        } catch (error) {
            console.error(error);
            throw error;
        }
    },

    getPagingBooking: async (
        req: GetPagingBookingRequest
    ): Promise<PaginatedResponse<CarBookingResponse[]>> => {
        try {
            const token = localStorage.getItem('token');
            const response = await axios.get<PaginatedResponse<CarBookingResponse[]>>(
                '/carBooking/Booking/paging',
                {
                    params: req,
                    headers: { Authorization: `Bearer ${token}` }
                }
            );
            return response.data;
        } catch (error) {
            console.error(error);
            throw error;
        }
    },

    updateBooking: async(req: UpdateBookingRequest): Promise<CarBookingResponse> => {
        try{
            const token = localStorage.getItem('token');
            const response = await axios.put<CarBookingResponse>('/carBooking/Booking', req, {headers: {Authorization: `Bearer ${token}`}});

            return response.data;
        }
        catch(error){
            console.error(error);
            throw error;
        }
    },

    deleteBooking: async (bookingId: number): Promise<void> => {
        try{
            const token = localStorage.getItem('token');
            await axios.delete(`/carBooking/booking/${bookingId}`, {headers: {Authorization: `Bearer ${token}`}});
        }
        catch(error){
            console.error(error);
            throw error;
        }
      }
};

export default bookingApiConnector;