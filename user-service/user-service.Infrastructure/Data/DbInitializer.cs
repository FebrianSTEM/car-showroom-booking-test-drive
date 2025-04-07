using user_service.Infrastructure.Data.Context;
using user_service.Infrastructure.Authentication;
using user_service.Domain.Entities;

namespace user_service.Infrastructure.Data
{
    public static class DbInitializer
    {
        private static PasswordHasher hasher = new PasswordHasher();

        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            // 1. Seed Roles
            var roles = new List<Role>
                    {
                        new Role("Admin") { Id = Guid.Parse("8D04DCE2-969A-435D-BBB4-DF3F325983DB") },
                        new Role("User") { Id = Guid.Parse("C7B013F0-5201-4317-ABD8-C211F91B7330") }
                    };
            context.Roles.AddRange(roles);
            context.SaveChanges();

            // 2. Create users
            var adminUser = new User(
                email: "admin@mail.com",
                phoneNumber: "080989999",
                passwordHash: hasher.HashPassword("Admin123")
            );
            adminUser.ConfirmEmail();
            adminUser.ConfirmPhoneNumber();

            var normalUser = new User(
                email: "user@mail.com",
                phoneNumber: "085752082822",
                passwordHash: hasher.HashPassword("User123")
            );
            normalUser.ConfirmEmail();
            normalUser.ConfirmPhoneNumber();

            // 3. Assign roles
            var adminRole = roles.First(r => r.Name == "Admin");
            var userRole = roles.First(r => r.Name == "User");

            adminUser.AddRole(adminRole);
            normalUser.AddRole(userRole);

            // 4. Save users (EF Core should track UserRoles automatically)
            context.Users.AddRange(adminUser, normalUser);
            context.SaveChanges();
        }
    }
}
