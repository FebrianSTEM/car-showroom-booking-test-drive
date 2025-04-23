using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Application.Models.Requests;
using user_service.Application.Models.Responses;
using user_service.Application.Services.Interfaces;
using user_service.Domain.Entities;
using user_service.Domain.Interfaces;

namespace user_service.Application.Services.Implementations
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtGenerator _jwtGenerator;

        public AccountService(
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IPasswordHasher passwordHasher,
            IJwtGenerator jwtGenerator)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordHasher = passwordHasher;
            _jwtGenerator = jwtGenerator;
        }

        public async Task<UserResponse> RegisterAsync(RegisterUserRequest registerDto)
        {
            // Validate input
            if (string.IsNullOrEmpty(registerDto.Email))
                throw new ArgumentException("Email is required");

            if (string.IsNullOrEmpty(registerDto.Password))
                throw new ArgumentException("Password is required");

            // Check if email already exists
            if (await _userRepository.EmailExistsAsync(registerDto.Email))
                throw new InvalidOperationException("Email is already in use");

            // Check if phone number already exists (if provided)
            if (!string.IsNullOrEmpty(registerDto.PhoneNumber) &&
                await _userRepository.PhoneNumberExistsAsync(registerDto.PhoneNumber))
                throw new InvalidOperationException("Phone number is already in use");

            // Hash password
            string passwordHash = _passwordHasher.HashPassword(registerDto.Password);

            // Create user
            var user = new User(registerDto.Email, registerDto.PhoneNumber, passwordHash);

            // Add roles (if provided)
            if (registerDto.Roles != null && registerDto.Roles.Any())
            {
                foreach (var roleName in registerDto.Roles)
                {
                    var role = await _roleRepository.GetByNameAsync(roleName);
                    if (role != null)
                    {
                        user.AddRole(role);
                    }
                }
            }
            else
            {
                // Add default role if no roles specified
                var defaultRole = await _roleRepository.GetByNameAsync("User");
                if (defaultRole != null)
                {
                    user.AddRole(defaultRole);
                }
            }

            // Save user
            await _userRepository.AddAsync(user);

            // Get user roles
            var userRoles = await _userRepository.GetUserRolesAsync(user);

            // Return user response
            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                CreatedAt = user.CreatedAt,
                Roles = userRoles.ToList()
            };
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest loginDto)
        {
            // Find user by email
            var user = await _userRepository.GetByEmailAsync(loginDto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid email or password");

            // Verify password
            if (!_passwordHasher.VerifyPassword(user.PasswordHash, loginDto.Password))
                throw new InvalidOperationException("Invalid email or password");

            // Update last login
            user.UpdateLastLogin();
            await _userRepository.UpdateAsync(user);

            // Get user roles
            var userRoles = await _userRepository.GetUserRolesAsync(user);

            // Generate JWT token with roles
            var token = _jwtGenerator.CreateToken(user.Id, user.Email, userRoles);

            // Return authentication response
            return new AuthResponse
            {
                Token = token,
                User = new UserResponse
                {
                    Id = user.Id,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    CreatedAt = user.CreatedAt,
                    Roles = userRoles.ToList()
                }
            };
        }

        public async Task<UserResponse> GetUserByIdAsync(Guid id)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var userRoles = await _userRepository.GetUserRolesAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                CreatedAt = user.CreatedAt,
                Roles = userRoles.ToList()
            };
        }

        public async Task<List<UserResponse>> GetUserByIdsAsync(List<Guid> userIds)
        {
            var user = await _userRepository.GetByIdsAsync(userIds);

            return user.Select(u => new UserResponse()
            {
                Id = u.Id,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                EmailConfirmed = u.EmailConfirmed,
                PhoneNumberConfirmed = u.PhoneNumberConfirmed,
                CreatedAt = u.CreatedAt
            }).ToList();
        }

        public async Task ConfirmEmailAsync(Guid userId, string token)
        {
            // In a real app, you would validate the token
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.ConfirmEmail();
            await _userRepository.UpdateAsync(user);
        }

        public async Task ConfirmPhoneNumberAsync(Guid userId, string token)
        {
            // In a real app, you would validate the token
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            user.ConfirmPhoneNumber();
            await _userRepository.UpdateAsync(user);
        }

        public async Task AddUserToRoleAsync(Guid userId, string roleName)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var role = await _roleRepository.GetByNameAsync(roleName);

            if (role == null)
                throw new KeyNotFoundException("Role not found");

            user.AddRole(role);
            await _userRepository.UpdateAsync(user);
        }

        public async Task RemoveUserFromRoleAsync(Guid userId, string roleName)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            var role = await _roleRepository.GetByNameAsync(roleName);

            if (role == null)
                throw new KeyNotFoundException("Role not found");

            user.RemoveRole(role);
            await _userRepository.UpdateAsync(user);
        }

        public async Task<IList<string>> GetUserRolesAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new KeyNotFoundException("User not found");

            return await _userRepository.GetUserRolesAsync(user);
        }
    }
}
