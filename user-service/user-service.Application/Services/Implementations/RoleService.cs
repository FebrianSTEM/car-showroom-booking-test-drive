using user_service.Application.Models.Responses;
using user_service.Application.Services.Interfaces;
using user_service.Domain.Entities;
using user_service.Domain.Interfaces;

namespace user_service.Application.Services.Implementations
{
    public class RoleService : IRoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IEnumerable<RoleResponse>> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            return roles.Select(x => new RoleResponse()
            {
                Name = x.Name
            }).AsEnumerable();
        }

        public async Task CreateRoleAsync(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role name is required");

            if (await _roleRepository.RoleExistsAsync(roleName))
                throw new InvalidOperationException($"Role '{roleName}' already exists");

            var role = new Role(roleName);
            await _roleRepository.AddAsync(role);
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            throw new NotImplementedException("Role deletion requires additional logic to handle users with this role");
        }
    }
}