using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos.Guides
{
    public class AddGuideDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(1000, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Description { get; set; }

        [Required] public IFormFile Image { get; set; }
    }
}