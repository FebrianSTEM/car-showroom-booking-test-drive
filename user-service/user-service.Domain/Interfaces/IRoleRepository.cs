using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Domain.Entities;

namespace user_service.Domain.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role> GetByNameAsync(string name);
        Task<bool> RoleExistsAsync(string name);
        Task AddAsync(Role role);
        Task<IEnumerable<Role>> GetAllRolesAsync();
    }
}
