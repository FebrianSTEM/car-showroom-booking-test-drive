using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace user_service.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }
        public string PasswordHash { get; private set; }
        public bool EmailConfirmed { get; private set; }
        public bool PhoneNumberConfirmed { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? LastLoginAt { get; private set; }

        // Role support
        public ICollection<UserRole> UserRoles { get; private set; } = new List<UserRole>();

        private User() { } // For EF Core

        public User(string email, string phoneNumber, string passwordHash)
        {
            Id = Guid.NewGuid();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            PhoneNumber = phoneNumber;
            PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
            EmailConfirmed = false;
            PhoneNumberConfirmed = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void ConfirmEmail()
        {
            EmailConfirmed = true;
        }

        public void ConfirmPhoneNumber()
        {
            PhoneNumberConfirmed = true;
        }

        public void UpdateLastLogin()
        {
            LastLoginAt = DateTime.UtcNow;
        }

        public void AddRole(Role role)
        {
            if (UserRoles.Any(ur => ur.RoleId == role.Id))
                return;

            UserRoles.Add(new UserRole
            {
                UserId = Id,
                RoleId = role.Id
            });
        }

        public void RemoveRole(Role role)
        {
            var userRole = UserRoles.FirstOrDefault(ur => ur.RoleId == role.Id);
            if (userRole != null)
            {
                UserRoles.Remove(userRole);
            }
        }
    }
}