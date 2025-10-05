using System.ComponentModel.DataAnnotations;


namespace Malama.Models
{
    //public class LoginViewModel
    //{
    //    public string Username { get; set; }
    //    public string Password { get; set; }
    //}


    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Remember Me")]
        public bool RememberMe { get; set; }
    }

}
