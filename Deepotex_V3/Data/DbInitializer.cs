using Deepotex.core.Models;
using Microsoft.AspNetCore.Identity;

namespace Deepotex_V2.Data
{
    public static class DbInitializer
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Ensure Database is created (optional, usually handled by migration)
            // context.Database.EnsureCreated();

            // Seed Admin User
            string adminEmail = "admin@deepotex.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Administrator",
                    EmailConfirmed = true
                };
                // Password: Admin@123
                await userManager.CreateAsync(adminUser, "Admin@123");
            }
        }
    }
}
