using EShop.Data;
using EShop.Repositories.Interface;

namespace EShop.Services
{
    public static class RoleSeeder
    {
        public static async Task SeedRolesAsync(IRoleRepository roleRepository)
        {
                var existingRoles = await roleRepository.GetAllAsync(CancellationToken.None);

                if (!existingRoles.Any(r => r.RoleName == "Admin"))
                    await roleRepository.CreateAsync(new Role { Id = Guid.NewGuid(), RoleName = "Admin" }, CancellationToken.None);

                if (!existingRoles.Any(r => r.RoleName == "User"))
                    await roleRepository.CreateAsync(new Role { Id = Guid.NewGuid(), RoleName = "User" }, CancellationToken.None);
        }
    }
}
