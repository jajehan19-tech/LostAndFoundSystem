using System.ComponentModel.DataAnnotations;

namespace LostAndFoundSystem.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
    }
}