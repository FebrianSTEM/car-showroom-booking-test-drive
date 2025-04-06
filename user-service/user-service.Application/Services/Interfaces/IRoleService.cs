using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Application.Models.Responses;

namespace user_service.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleResponse>> GetAllRolesAsync();
        Task CreateRoleAsync(string roleName);
        Task DeleteRoleAsync(string roleName);
    }
}
