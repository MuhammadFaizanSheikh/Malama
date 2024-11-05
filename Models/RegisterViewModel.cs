using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ExcelFilesCompiler.Models
{
    public class RegisterViewModel : IdentityUser
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } // Admin or User
        public bool IsActive { get; set; }
    }
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true; // Default to active
    }
}
