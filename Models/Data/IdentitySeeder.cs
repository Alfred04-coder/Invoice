using InvoiceManagement.Models.Data.InvoiceDbContext;
using Microsoft.AspNetCore.Identity;
using InvoiceManagement.Models;

namespace InvoiceManagement.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles =
            {
                "SystemAdmin",
                "Admin",
                "Cashier"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedSystemAdmin(
            UserManager<ApplicationUser> userManager)
        {
            string email = "admin@system.com";

            if (await userManager.FindByEmailAsync(email) != null)
                return;

            var admin = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,

                // Temporary value for the platform admin
                CompanyId = null,
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "SystemAdmin");
            }
        }
    }
}