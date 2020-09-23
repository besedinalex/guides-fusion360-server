using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos.Users
{
    public class UserRegisterDto
    {
        [Required, EmailAddress] public string Email { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required]
        [StringLength(52, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 8)]
        public string Password { get; set; }
    }
}