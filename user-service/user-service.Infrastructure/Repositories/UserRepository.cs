using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using user_service.Domain.Entities;
using user_service.Domain.Interfaces;
using user_service.Infrastructure.Data.Context;

namespace user_service.Infrastructure.Repositories
{
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User> GetByIdAsync(Guid id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> PhoneNumberExistsAsync(string phoneNumber)
        {
            return await _context.Users.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<string>> GetUserRolesAsync(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            if (user.UserRoles == null || !user.UserRoles.Any())
            {
                var dbUser = await _context.Users
                                           .Include(u => u.UserRoles)
                                           .ThenInclude(ur => ur.Role)
                                           .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (dbUser != null)
                {
                    return dbUser.UserRoles.Select(ur => ur.Role.Name).ToList();
                }

                return new List<string>();
            }

            return user.UserRoles.Select(ur => ur.Role.Name).ToList();
        }
    }
}
