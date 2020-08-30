using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos
{
    public class AddNewGuideModelDto
    {
        [Required] public int GuideId { get; set; }

        [Required] public IFormFile File { get; set; }
    }
}