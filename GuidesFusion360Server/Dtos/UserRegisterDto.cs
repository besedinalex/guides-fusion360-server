using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos
{
    public class UserRegisterDto
    {
        [Required] public string Email { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Password { get; set; }
    }
}