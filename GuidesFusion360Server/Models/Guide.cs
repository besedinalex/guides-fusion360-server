using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class Guide
    {
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        [Required]
        public int OwnerId { get; set; }
        
        [Required]
        public string Hidden { get; set; }
    }
}
