using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos.Guides
{
    public class AddPartGuideDto
    {
        [Required]
        [StringLength(100, ErrorMessage = "{0} length must be between {2} and {1}.", MinimumLength = 2)]
        public string Name { get; set; }

        public string Content { get; set; }

        [Required] public int? SortKey { get; set; }

        [Required] public int? GuideId { get; set; }

        public IFormFile File { get; set; }
    }
}