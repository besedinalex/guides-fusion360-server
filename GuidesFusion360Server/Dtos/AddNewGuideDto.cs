using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Dtos
{
    public class AddNewGuideDto
    {
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Description { get; set; }
    }
}
