using car_booking_service.Application.Models.Requests.UserServiceRequests;
using car_booking_service.Application.Models.Responses.UserServiceResponses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace car_booking_service.Application.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserResponse>> GetUserByIds(List<string> request);
    }
}
