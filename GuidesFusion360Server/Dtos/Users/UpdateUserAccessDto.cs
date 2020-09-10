using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos.Users
{
    public class UpdateUserAccessDto
    {
        [Required] public string Email { get; set; }

        [Required] public string Access { get; set; }
    }
}