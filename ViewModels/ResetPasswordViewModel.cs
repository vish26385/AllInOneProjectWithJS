using System.ComponentModel.DataAnnotations;

namespace AllInOneProject.ViewModels
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string? Token { get; set; }
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
