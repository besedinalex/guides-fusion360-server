using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos.Guides
{
    public class AddGuideDto
    {
        [Required] public string Name { get; set; }

        [Required] public string Description { get; set; }

        [Required] public IFormFile Image { get; set; }
    }
}