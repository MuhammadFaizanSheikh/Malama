using System.ComponentModel.DataAnnotations;

namespace Malama.Models
{
    public class Verify2FAViewModel
    {
        [Required]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }
}
