using System.ComponentModel.DataAnnotations;

namespace AllInOneProject.DTOs
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "User name is required")]
        [MaxLength(100)]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
