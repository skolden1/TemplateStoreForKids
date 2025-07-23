using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MallarEmelieMVC.Areas.Identity.Models
{
    public class IdentityUserTable : IdentityUser
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        public string? ProfilePicture { get; set; }

        public DateTime DateJoined { get; set; } = DateTime.Now;

        
    }
}
