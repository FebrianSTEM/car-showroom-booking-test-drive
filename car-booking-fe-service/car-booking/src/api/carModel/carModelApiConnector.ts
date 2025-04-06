import axios from 'axios';
import { PaginatedCarModelRequest } from '../../models/request/carModel/paginatedCarModelRequest';
import { CarModelResponse } from '../../models/response/carModel/carModelResponse';
import { PaginatedResponse } from '../../models/paginatedResponse';
import { UpdateCarModelRequest } from '../../models/request/carModel/updateCarModelRequest';
import { CreateCarModelRequest } from '../../models/request/carModel/createCarModelRequest';

const carModelApiConnector = {

    getPaginatedCarModels: async (req: PaginatedCarModelRequest): Promise<PaginatedResponse<CarModelResponse[]>> => {
        try {
            const response: PaginatedResponse<CarModelResponse[]> = await axios.get(`/carBooking/CarModels/paging`, 
                {
                    params: req
                });
            return response;
        }
        catch (error) {
            console.log(error);
            throw error;
        }
    },

    getCarModelById: async (carId: number): Promise<CarModelResponse> => {
        try{
            const response: CarModelResponse = await axios.get(`/carBooking/CarModels/${carId}`);
            return response;
        }
        catch(error){
            console.log(error);
            throw error;
        }
    },

    deleteCarById: async(carId: number): Promise<CarModelResponse> => {
        try{
            const token = localStorage.getItem('token');
            const response: CarModelResponse = await axios.delete(`/carBooking/CarModels/${carId}`, 
                                                                  {headers: { Authorization: `Bearer ${token}`}});
            return response;
        }
        catch(error){
            console.log(error);
            throw error;
        }
    },

    editCarById: async(req: UpdateCarModelRequest): Promise<CarModelResponse> => {
        try{
            const token = localStorage.getItem('token');
            const response: CarModelResponse = await axios.put(`/carBooking/CarModels/`, 
                req,
                {headers: { Authorization: `Bearer ${token}`}}
            );
            return response;
        }
        catch(error){
            console.log(error);
            throw error;
        }
    },

    createCarModel: async(req: CreateCarModelRequest): Promise<CarModelResponse> => {
        try{
            const token = localStorage.getItem('token');
            const response: CarModelResponse = await axios.post('/carBooking/CarModels', 
                req, 
                {headers: { Authorization: `Bearer ${token}`}}
            );
            return response;
        }
        catch(error){
            console.log(error);
            throw error;
        }
    }
}

export default carModelApiConnector;