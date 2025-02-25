using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Newtonsoft.Json;
using System.Security.Policy;

namespace ExcelFilesCompiler
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, WebApplicationBuilder builder, IWebHostEnvironment env)
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

            var rolesFilePath = Path.Combine(env.WebRootPath, "data", "roles.json");

            if (File.Exists(rolesFilePath))
            {
                // Read the JSON file
                var json = await File.ReadAllTextAsync(rolesFilePath);

                // Deserialize into a list of RoleData objects
                var roleDataList = JsonConvert.DeserializeObject<List<RoleData>>(json);

                if (roleDataList != null)
                {
                    // Filter the roles where IsAdditionalRole is false
                    var filteredRoleDataList = roleDataList.Where(roleData => !roleData.IsAdditionalRole).ToList();

                    foreach (var roleData in filteredRoleDataList)
                    {
                        string roleName = roleData.Value; // Extract "value" field as the role name

                        // Check if the role exists and create if it doesn't
                        if (!await roleManager.RoleExistsAsync(roleName))
                        {
                            await roleManager.CreateAsync(new IdentityRole(roleName));
                        }
                    }
                }
            }
            else
            {
                // Log or handle the missing file case
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
                    TwoFactorEnabled = false, // Admin should have 2FA enabled, you can adjust this if needed
                    IsEventUser = false
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

    public class RoleData
    {
        public string Id { get; set; }
        public string Value { get; set; }
        public bool IsAdditionalRole { get; set; }
        public List<string> Types { get; set; }
    }

}
