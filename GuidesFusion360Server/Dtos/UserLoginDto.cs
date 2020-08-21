using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos
{
    public class UserLoginDto
    {
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
    }
}
