using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }

        public async Task<IActionResult> GetRolesByCategory(string category)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(category))
                {
                    return BadRequest(new { message = "Category parameter is required." });
                }

                var roles = _roleManager.Roles
    .Where(r => r.Category == category) // Filter roles by category
    .Select(r => new { r.Id, r.Name, r.Types })
    .ToList(); // Use ToList() instead of ToListAsync()


                if (roles == null || !roles.Any())
                {
                    return NotFound(new { message = "No roles found for the given category." });
                }

                return Ok(roles); // Return roles as JSON array
            }
            catch (Exception ex)
            {
                // Log the error (You can replace this with a proper logging framework like Serilog)

                return StatusCode(500, new { message = "An error occurred while fetching roles. Please try again later." });
            }
        }

    }
}
