using System.ComponentModel.DataAnnotations;

namespace AllInOneProject.DTOs
{
    public class LoginResultDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
