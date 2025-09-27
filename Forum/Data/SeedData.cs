using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Forum.Models;

namespace Forum.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            RoleManager<IdentityRole> roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            bool adminRoleExists = await roleManager.RoleExistsAsync("admin");
            if (adminRoleExists == false)
            {
                await roleManager.CreateAsync(new IdentityRole("admin"));
            }

            bool userRoleExists = await roleManager.RoleExistsAsync("user");
            if (userRoleExists == false)
            {
                await roleManager.CreateAsync(new IdentityRole("user"));
            }

            ApplicationUser adminUser = await userManager.FindByEmailAsync("admin@forum.local");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@forum.local",
                    AvatarPath = "/uploads/avatars/default_admin.png"
                };
                IdentityResult result = await userManager.CreateAsync(adminUser, "Admin1!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "admin");
                }
            }
        }
    }
}