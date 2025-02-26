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

        [Required(ErrorMessage = "At least one role must be selected.")]
        [MinLength(1, ErrorMessage = "At least one role must be selected.")]
        public List<string> SelectedRoles { get; set; }
        public bool IsActive { get; set; }
    }
    public class ApplicationUser : IdentityUser
    {
        public bool IsActive { get; set; } = true; // Default to active
        public bool IsEventUser { get; set; } = false; // Default to active
    }

    public class UserUpdateDto
    {
        public string Id { get; set; }         // User's unique ID
        [Required]
        [EmailAddress]
        public string Email { get; set; }      // User's email address
        [Required]
        public string Password { get; set; }   // New password (if applicable)
        [Required(ErrorMessage = "At least one role must be selected.")]
        [MinLength(1, ErrorMessage = "At least one role must be selected.")]
        public List<string> SelectedRoles { get; set; }      // Role (e.g., "User", "Admin")
    }

    public class ApplicationRole : IdentityRole
    {
        public string Category { get; set; } // New property for categorization
        public string Types { get; set; }
    }
}
