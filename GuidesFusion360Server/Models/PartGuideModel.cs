using System.ComponentModel.DataAnnotations;

namespace GuidesFusion360Server.Models
{
    public class PartGuideModel
    {
        public int Id { get; set; }

        [Required] public string Name { get; set; }

        [Required] public string Content { get; set; }

        [Required] public int SortKey { get; set; }

        [Required] public int GuideId { get; set; }
    }
}