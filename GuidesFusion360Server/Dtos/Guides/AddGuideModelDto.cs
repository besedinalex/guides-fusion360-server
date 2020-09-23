using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos.Guides
{
    public class AddGuideModelDto
    {
        [Required] public int? GuideId { get; set; }

        [Required] public IFormFile File { get; set; }
    }
}