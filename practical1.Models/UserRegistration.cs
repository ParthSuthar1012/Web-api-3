using System.ComponentModel.DataAnnotations;

namespace practical1.Models
{
    public class UserRegistration
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
