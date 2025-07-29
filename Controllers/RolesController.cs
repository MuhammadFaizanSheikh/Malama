using ExcelFilesCompiler.Interfaces;
using ExcelFilesCompiler.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelFilesCompiler.Controllers
{
    [Authorize]
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService ?? throw new ArgumentNullException(nameof(roleService));
        }

        [HttpGet]
        public async Task<IActionResult> GetRolesByCategory(string category)
        {
            try
            {
                var roles = await _roleService.GetRolesByCategoryAsync(category);

                if (!roles.Any())
                {
                    return NotFound(new { message = "No roles found for the given category." });
                }

                return Ok(roles.Select(r => new { r.Id, r.Name, r.Types, r.Category }));
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { message = "An error occurred while fetching roles. Please try again later." });
            }
        }
    }
}
