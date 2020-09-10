using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class PartGuide
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Content { get; set; }

        [Required] public int SortKey { get; set; }

        public int GuideId { get; set; }
        
        public Guide Guide { get; set; }
    }
}