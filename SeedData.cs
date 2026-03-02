using Malama.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ExcelFilesCompiler
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider, WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            // Create a scope to access scoped services like DbContext, RoleManager, etc.
            using var scope = serviceProvider.CreateScope();
            var scopedProvider = scope.ServiceProvider;

            // Call individual seeders
            await SeedRolesAndAdminAsync(scopedProvider, builder, env);
            await SeedContainersAsync(scopedProvider, env);
        }

        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider, WebApplicationBuilder builder, IWebHostEnvironment env)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>(); // Ensure ApplicationRole is used
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var rolesFilePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "roles.json");

            if (File.Exists(rolesFilePath))
            {
                // Read the JSON file
                var json = await File.ReadAllTextAsync(rolesFilePath);

                // Deserialize into a list of RoleData objects
                var roleDataList = JsonConvert.DeserializeObject<List<RoleData>>(json);

                if (roleDataList != null)
                {
                    foreach (var roleData in roleDataList)
                    {
                        string roleName = roleData.Value; // Extract role name
                        string category = roleData.Category; // Extract category
                        string types = roleData.Types != null ? string.Join(",", roleData.Types) : string.Empty; // Convert list to CSV

                        // Check if the role exists by fetching it from the database
                        var existingRole = await roleManager.FindByNameAsync(roleName);

                        if (existingRole == null)
                        {
                            // Create new role with Category and Types
                            var role = new ApplicationRole
                            {
                                Name = roleName,
                                Category = category,
                                Types = types
                            };

                            await roleManager.CreateAsync(role);
                        }
                        else
                        {
                            // If role exists, update its Category and Types if they are different
                            bool isUpdated = false;

                            if (existingRole.Category != category)
                            {
                                existingRole.Category = category;
                                isUpdated = true;
                            }

                            if (existingRole.Types != types)
                            {
                                existingRole.Types = types;
                                isUpdated = true;
                            }

                            if (isUpdated)
                            {
                                await roleManager.UpdateAsync(existingRole);
                            }
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
                    IsEventUser = false,
                    IsSuperAdmin = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // Assign the Admin role to the user
                    await userManager.AddToRoleAsync(adminUser, "Super Admin");

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

        public static async Task SeedContainersAsync(IServiceProvider serviceProvider, IWebHostEnvironment env)
        {
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Skip if already seeded
            if (await dbContext.ContainerType.AnyAsync())
                return;

            var filePath = Path.Combine(env.ContentRootPath, "Data", "Seed", "containers.json");

            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file not found: {filePath}");
                return;
            }

            var json = await File.ReadAllTextAsync(filePath);
            var containers = JsonConvert.DeserializeObject<List<ContainerType>>(json);

            if (containers != null && containers.Count > 0)
            {
                foreach (var container in containers)
                {
                    container.AddedBy = "SystemSeeder";
                    container.AddedOn = DateTime.Now;
                    container.UpdatedBy = null;
                    container.UpdatedOn = null;
                }

                await dbContext.ContainerType.AddRangeAsync(containers);
                await dbContext.SaveChangesAsync();
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
        public string Category { get; set; }
        public List<string> Types { get; set; }
    }

}
