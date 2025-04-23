using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Application.Models.Requests;
using user_service.Application.Models.Responses;

namespace user_service.Application.Services.Interfaces
{
    public interface IAccountService
    {
        Task<UserResponse> RegisterAsync(RegisterUserRequest registerDto);
        Task<AuthResponse> LoginAsync(LoginRequest loginDto);
        Task<UserResponse> GetUserByIdAsync(Guid id);
        Task<List<UserResponse>> GetUserByIdsAsync(List<Guid> userIds);
        Task ConfirmEmailAsync(Guid userId, string token);
        Task ConfirmPhoneNumberAsync(Guid userId, string token);
        Task AddUserToRoleAsync(Guid userId, string roleName);
        Task RemoveUserFromRoleAsync(Guid userId, string roleName);
        Task<IList<string>> GetUserRolesAsync(Guid userId);
    }
}
