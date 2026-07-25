using InventoryManagement.Application.Common;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagement.Persistence.Identity;

public static class IdentitySeeder
{
    public static async Task SeedAsync(
        UserManager<AppUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        if(!await roleManager.RoleExistsAsync(RoleNames.Admin))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Admin));
        }

        if(!await roleManager.RoleExistsAsync(RoleNames.Customer))
        {
            await roleManager.CreateAsync(new IdentityRole(RoleNames.Customer));
        }

        var adminUser = await userManager.FindByEmailAsync("admin@inventory.com");

        if (adminUser is null)
        {
            adminUser = new AppUser
            {
                FullName = "System Administrator",
                UserName = "admin",
                Email = "admin@inventory.com",
                CreatedDate = DateTime.UtcNow,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(
                adminUser,
                "Admin123!");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, RoleNames.Admin);
            }


        }
    }
}