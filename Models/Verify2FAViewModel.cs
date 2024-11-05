using System.ComponentModel.DataAnnotations;

namespace ExcelFilesCompiler.Models
{
    public class Verify2FAViewModel
    {
        [Required]
        [Display(Name = "Verification Code")]
        public string Code { get; set; }
    }
}
