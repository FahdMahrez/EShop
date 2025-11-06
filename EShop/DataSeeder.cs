using EShop.Context;
using EShop.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace EShop
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            Console.WriteLine(">>> Starting DataSeeder.SeedAdminAsync...");
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            await context.Database.MigrateAsync();

            var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Admin");
            if (adminRole == null)
            {
                adminRole = new Role
                {
                    Id = Guid.NewGuid(),
                    RoleName = "Admin"
                };
                context.Roles.Add(adminRole);
                await context.SaveChangesAsync();
            }

            bool adminExists = await context.UserRoles
                .Include(ur => ur.Role)
                .Include(ur => ur.User)
                .AnyAsync(ur => ur.Role.RoleName == "Admin");

            if (!adminExists)
            {
                // Step 3: Create the default admin user
                var adminUser = new User
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Super",
                    LastName = "Admin",
                    Email = "admin1@gmail.com",
                    PasswordHash = HashPassword("Admin@123"),
                    PhoneNumber = "0000000000",
                    Address = "System Default",
                    Gender = Gender.Male
                };

                context.Users.Add(adminUser);
                await context.SaveChangesAsync();

                // Step 4: Assign Admin Role
                var userRole = new UserRole
                {
                    Id = Guid.NewGuid(),
                    UserId = adminUser.Id,
                    RoleId = adminRole.Id
                };

                context.UserRoles.Add(userRole);
                await context.SaveChangesAsync();

                Console.WriteLine("Default Admin created successfully!");
                Console.WriteLine("Email: admin1@gmail.com");
                Console.WriteLine("Password: Admin@123");
            }
            else
            {
                Console.WriteLine("Admin already exists — skipping seeding.");
            }
        }

        private static string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }
    }
}
