using Microsoft.AspNetCore.Identity;

namespace AllInOneProject.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}
