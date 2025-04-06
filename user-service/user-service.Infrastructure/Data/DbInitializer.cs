using user_service.Infrastructure.Data.Context;
using user_service.Domain.Entities;

namespace user_service.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            if (context.Users.Any()) return;

            var roles = new List<Role>
            {
                new Role("Admin") { Id = Guid.Parse("8D04DCE2-969A-435D-BBB4-DF3F325983DB") },
                new Role("User") { Id = Guid.Parse("C7B013F0-5201-4317-ABD8-C211F91B7330") }
            };

            context.Roles.AddRange(roles);
            context.SaveChanges();
        }
    }
}
