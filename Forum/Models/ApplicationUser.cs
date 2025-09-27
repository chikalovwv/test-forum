using Microsoft.AspNetCore.Identity;

namespace Forum.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarPath { get; set; }
        public bool IsBlocked { get; set; }
    }
}