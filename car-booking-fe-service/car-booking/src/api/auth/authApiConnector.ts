import axios from 'axios';
import { RegisterUserRequest } from '../../models/request/registerUserRequest';
// import { LoginRequest } from '../../models/request/loginRequest';
// import { UserRoleRequest } from '../../models/request/userRoleRequest';
import { StandardResponse } from '../../models/standardResponse';
// import { UserResponse } from '../../models/response/userResponse';
import { AuthResponse } from '../../models/response/authResponse';
import { LoginRequest } from '../../models/request/auth/loginRequest';
// import { RoleResponse } from '../../models/response/roleResponse';

const authApiConnector = {

    registerUser: async (req: RegisterUserRequest): Promise<AuthResponse> => {
        try {
            if (!req.roles) {
                req.roles = [];
            }
            req.roles = [...(req.roles || []), "User"];
            const response: StandardResponse<AuthResponse> = await axios.post(`/auth/Account/register`, req);
            const authResponse = response.data;
            return authResponse;
        }
        catch (error) {
            console.log(error);
            throw error;
        }
    },

    loginUser: async (req: LoginRequest): Promise<AuthResponse> => {
        try{
            const response: StandardResponse<AuthResponse> = await axios.post(`/auth/Account/login`, req);
            const authResponse = response.data;
            return authResponse;
        }
        catch (error) {
            console.log(error);
            throw error;
        }
    }
}

export default authApiConnector;