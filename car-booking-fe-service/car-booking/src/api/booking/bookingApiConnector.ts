import axios from 'axios';
import { StandardResponse } from '../../models/standardResponse';
import { CarBookingRequest } from '../../models/request/booking/createBookingRequest';
import { CarBookingResponse } from '../../models/response/booking/bookingResponse';

const bookingApiConnector = {
    createBooking: async (req: CarBookingRequest): Promise<CarBookingResponse> => {
        try{
            const token = localStorage.getItem('token');
            const response: StandardResponse<CarBookingResponse> = await axios.post(`/carBooking/Booking`, req, {headers: { Authorization: `Bearer ${token}`}});
            const authResponse = response.data;
            return authResponse;
        }
        catch (error) {
            console.log(error);
            throw error;
        }
    }
}

export default bookingApiConnector;