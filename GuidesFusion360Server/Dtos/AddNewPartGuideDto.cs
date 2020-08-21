using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace GuidesFusion360Server.Dtos
{
    public class AddNewPartGuideDto
    {
        [Required]
        public string Name { get; set; }
        
        public string Content { get; set; }
        
        [Required]
        public int SortKey { get; set; }
        
        [Required]
        public int GuideId { get; set; }
        
        public IFormFile File { get; set; }
    }
}
