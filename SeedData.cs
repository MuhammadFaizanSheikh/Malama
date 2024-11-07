using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Security.Policy;

namespace ExcelFilesCompiler
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, WebApplicationBuilder builder)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // Ensure roles exist
            string[] roles = { "Admin", "User" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = builder.Configuration["AdminCredentials:AdminUser"];
            var adminPassword = builder.Configuration["AdminCredentials:AdminPassword"];

            // Create the admin user if it doesn't exist
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    IsActive = true, // You can set other properties as necessary
                    TwoFactorEnabled = false // Admin should have 2FA enabled, you can adjust this if needed
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Admin");

                    // Optionally, confirm the admin's email if you are bypassing email confirmation
                    await userManager.ConfirmEmailAsync(adminUser, await userManager.GenerateEmailConfirmationTokenAsync(adminUser));
                }
                else
                {
                    // Handle failure to create the user, maybe log it or throw an exception
                    throw new Exception("Failed to create the admin user.");
                }
            }
        }

        //public static async Task Initialize(IServiceProvider serviceProvider, WebApplicationBuilder builder)
        //{
        //    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //    var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        //    // Ensure roles exist
        //    string[] roles = { "Admin", "User" };
        //    foreach (var role in roles)
        //    {
        //        if (!await roleManager.RoleExistsAsync(role))
        //        {
        //            await roleManager.CreateAsync(new IdentityRole(role));
        //        }
        //    }

        //    var adminEmail = builder.Configuration["AdminCredentials:AdminUser"];
        //    var adminPassword = builder.Configuration["AdminCredentials:AdminPassword"];

        //    if (await userManager.FindByEmailAsync(adminEmail) == null)
        //    {
        //        var adminUser = new ApplicationUser { UserName = adminEmail, Email = adminEmail };
        //        var result = await userManager.CreateAsync(adminUser, adminPassword);

        //        if (result.Succeeded)
        //        {
        //            await userManager.AddToRoleAsync(adminUser, "Admin");
        //        }
        //    }
        //}
    }
}
