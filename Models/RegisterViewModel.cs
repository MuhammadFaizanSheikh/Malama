using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Malama.Models
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
        public List<long>? SelectedEventIds { get; set; }
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

    public class AccountUserListDto
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }

        public bool IsActive { get; set; }
        public bool IsEventManager { get; set; }

        public string Role { get; set; }
        public List<string> Events { get; set; } = new();

        public List<UserPagePermissionDto> AllowedPages { get; set; } = new();
    }

    public class UserPagePermissionDto
    {
        public string PageKey { get; set; }      // ContractDetails
        public string PageName { get; set; }     // Contract Details Register
        public PageAccessLevel Access { get; set; }
    }
}
