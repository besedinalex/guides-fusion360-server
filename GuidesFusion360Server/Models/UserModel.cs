using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required] public string Email { get; set; }

        [Required] public string FirstName { get; set; }

        [Required] public string LastName { get; set; }

        [Required] public string Access { get; set; }

        [Required] public byte[] PasswordHash { get; set; }

        [Required] public byte[] PasswordSalt { get; set; }
    }
}