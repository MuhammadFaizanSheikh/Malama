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

    public class UserUpdateDto
    {
        public string Id { get; set; }         // User's unique ID
        public string Email { get; set; }      // User's email address
        public string Password { get; set; }   // New password (if applicable)
        public string Role { get; set; }       // Role (e.g., "User", "Admin")
    }
}
